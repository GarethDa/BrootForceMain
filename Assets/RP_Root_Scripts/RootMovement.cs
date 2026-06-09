using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RootMovement : MonoBehaviour
{
    [Header("Movement Properties")]
    public float baseSpeed = 10f;
    public float maxSpeed = 30f;
    public float turnSpeed = 100f;
    public float accelTime = 3f;
    public float decelRatio = 2f;

    [Header("Line Rendering Properties")]
    public LineRenderer frontRenderer;
    public LineRenderer backRenderer;
    public float timerPerNode = 0.5f;
    public int numFrontNodes = 15;

    [Header("Obstacle Breaking Properties")]
    public TMP_Text breakingText;
    public float breakingStrength = 1;

    //Referenced components
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    //Variables for holding temp values
    private float poopTimer = 0f;
    private bool accelerating = false;
    private float currentSpeed;
    private float speedUpDownTimer = 0f;
    private int turnDirection = 0;
    private float currentObstacleStrength;
    private bool breakingObstacle = false;
    private bool breakThisFrame = false;
    private GameObject currentObstacle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        breakingText.gameObject.SetActive(false);

        //Start the front renderer with 2 nodes, both at the root starting position
        frontRenderer.positionCount = 2;
        frontRenderer.SetPosition(0, transform.position);
        frontRenderer.SetPosition(1, transform.position);

        //Start the back renderer with two nodes
        backRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        //Do input
        InputPass();

        //If we're breaking an obstacle, do the algorithm for that
        if (breakingObstacle)
        {
            if (breakThisFrame)
            {
                currentObstacleStrength -= breakingStrength;

                if (currentObstacleStrength <= 0)
                {
                    breakingText.gameObject.SetActive(false);
                    Destroy(currentObstacle);
                    breakingObstacle = false;
                }

                else
                {
                    breakingText.text = "Strength to break: " + currentObstacleStrength;
                }
            }
        }

        //If we're not breaking an obstacle, we'll update the line renderer
        else
        {
            //Update the node spawn timer
            poopTimer += Time.deltaTime;

            //Once the node timer reaches the threshold for spawning a new node, do the spawning stuff
            if (poopTimer >= timerPerNode)
            {
                //If the front renderer is at its maximum node count,
                //then we're gonna move nodes across to the back renderer
                if (frontRenderer.positionCount >= numFrontNodes)
                {
                    //Add a node to the back renderer, and set the three frontmost nodes to the backmost nodes of the front renderer.
                    //We do three instead of just one so that there are no visivble seams/cracks between the two renderers.
                    backRenderer.positionCount++;
                    backRenderer.SetPosition(backRenderer.positionCount - 3, frontRenderer.GetPosition(0));
                    backRenderer.SetPosition(backRenderer.positionCount - 2, frontRenderer.GetPosition(1));
                    backRenderer.SetPosition(backRenderer.positionCount - 1, frontRenderer.GetPosition(2));

                    //Go through all of the front renderer's nodes, up to the second one, and set their positions forward one node.
                    //Essentially, this moves all of the nodes up one position and leaves the backmost node to go to the back renderer.
                    for (int i = 0; i <= numFrontNodes - 2; i++)
                        frontRenderer.SetPosition(i, frontRenderer.GetPosition(i + 1));
                }

                //This "else" section only happens up until the front renderer reaches max nodes, then this is never reached again.
                //Add a new node to the front renderer
                else
                    frontRenderer.positionCount++;

                //Reset the timer
                poopTimer = 0f;
            }

            //Every frame, update the frontmost node to always be at the front of the root.
            //This guarantees that the front of the root is always connected to the rest of the root.
            frontRenderer.SetPosition(frontRenderer.positionCount - 1, transform.position);
        }
    }

    private void FixedUpdate()
    {
        //Only move the root if it's not currently breaking an obstacle
        if (!breakingObstacle)
        {
            //Turn the front of the root around the z-axis depending on the direction input.
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z + (turnDirection * turnSpeed) * Time.deltaTime
                );

            //If we are accelerating, either add to the speed timer or keep it at the maximum if it is already at the max.
            if (accelerating)
                speedUpDownTimer = Mathf.Min(speedUpDownTimer + Time.deltaTime, accelTime);

            //If we are decelerating, either subtract from the speed timer or keep it at zero if it already is zero.
            //Use the decelRatio, so that this decelerates faster/slower than it accelerates
            else
                speedUpDownTimer = Mathf.Max(speedUpDownTimer - decelRatio * Time.deltaTime, 0);

            //Lerp between the base and max speed, based on the timer value
            currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, speedUpDownTimer / accelTime);

            //Move the root forward, with a speed reduced by the biome's slowdown value
            rb.MovePosition(rb.position + (Vector2)(-transform.up * (currentSpeed * (1f - LevelManager.instance.GetBiomeSlowdown()))) * Time.deltaTime);
        }
    }

    private void InputPass()
    {
        if (playerInput.actions["Turn_Clockwise"].WasPressedThisFrame())
            turnDirection++;

        if (playerInput.actions["Turn_Counterclockwise"].WasPressedThisFrame())
            turnDirection--;

        if (playerInput.actions["Turn_Clockwise"].WasReleasedThisFrame())
            turnDirection--;

        if (playerInput.actions["Turn_Counterclockwise"].WasReleasedThisFrame())
            turnDirection++;

        if (playerInput.actions["Accelerate"].WasPressedThisFrame())
            accelerating = true;

        if (playerInput.actions["Accelerate"].WasReleasedThisFrame())
            accelerating = false;

        if (playerInput.actions["Break_Obstacle"].WasPressedThisFrame())
            breakThisFrame = true;

        else
            breakThisFrame = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided!");
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            Debug.Log("It a obstacle");
            breakingText.gameObject.SetActive(true);
            currentObstacleStrength = collision.gameObject.GetComponent<ObstacleValues>().breakStrength;
            breakingText.text = "Strength to break: " + currentObstacleStrength;
            breakingObstacle = true;
            currentObstacle = collision.gameObject;
        }
    }

    /*
    private void OnCol(Collision2D collision)
    {
        Debug.Log("Collided!");
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            Debug.Log("It a obstacle");
            breakingText.gameObject.SetActive(true);
            currentObstacleStrength = collision.gameObject.GetComponent<ObstacleValues>().breakStrength;
            breakingText.text = "Strength to break: " + currentObstacleStrength;
            breakingObstacle = true;
            currentObstacle = collision.gameObject;
        }
    }
    */

    private void OnCollisionStay2D(Collision2D collision)
    {

        Debug.Log("hit barrier");
        if (collision.gameObject.tag.Equals("Barrier"))
        {
            //Debug.Log("hit barrier");

            //Grab the normal, use this to find the line perpendicular to the collision 
            //(e.g. the line that runs along the barrier we are colliding with
            Vector2 hitNormal = collision.contacts[0].normal;
            Vector2 normalPerp = Vector2.Perpendicular(hitNormal).normalized;

            //Find the dot product of the perpendicular line and the root's up vector.
            float dotProd = Vector2.Dot(normalPerp, transform.up);

            //Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + normalPerp * 20f, Color.red, 2f);

            //If the dot product is positive or zero, this means the root is approaching the barrier at an angle <=90 degrees, measuring from the top.
            //Therefore, in this case, we want to rotate the root further downward (relative to the barrier).
            if (dotProd >= 0)
            {
                //Debug.Log("Going Down");

                //Find the direction that we want the root to turn towards. In this case, we subtract the sum of the perpendicular vector and the normal vector.
                //It's important to note that this doesn't need to be super precise, it just needs to be a vector which points in the general direction we want.
                //This is because, the moment the root isn't touching the barrier, the rotation stops anyway.
                Vector3 turnDir = (transform.position - ((Vector3)normalPerp + (Vector3)hitNormal));
               
                //Find the quaternion representing the rotation to turn and look at the target.
                Quaternion targetRot = Quaternion.LookRotation(turnDir);

                //Find the z-euler component of the rotation, since this is all we need.
                float targetEuler = targetRot.eulerAngles.z;
                
                //Debug.Log(targetEuler);

                //Turn the root at the pre-determined turn speed.
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    Mathf.MoveTowards(transform.eulerAngles.z, targetEuler, -turnSpeed * Time.deltaTime)
                    );
            }

            //If the dot product is negative, this means the root is approaching the barrier at an angle >90 degrees, measuring from the top.
            //Therefore, in this case, we want to rotate the root further upward (relative to the barrier).
            else
            {
                //Debug.Log("Going Up");

                //Same logic as above, but we are adding instead of subtracting since we want to go upwards relative to the barrier.
                Vector3 turnDir = (transform.position + ((Vector3)normalPerp + (0.5f * (Vector3)hitNormal)));
                Quaternion targetRot = Quaternion.LookRotation(turnDir);
                float targetEuler = targetRot.eulerAngles.z;

                //Debug.Log(targetEuler);

                //Turn the root at the pre-determined turn speed.
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    Mathf.MoveTowards(transform.eulerAngles.z, targetEuler, turnSpeed * Time.deltaTime)
                    );
            }
        }
    }
}
