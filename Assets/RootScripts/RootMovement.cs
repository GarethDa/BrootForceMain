using UnityEngine;
using UnityEngine.InputSystem;

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

    //Referenced components
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    //Variables for holding temp values
    private float poopTimer = 0f;
    private bool accelerating = false;
    private float currentSpeed;
    private float speedUpDownTimer = 0f;
    private int turnDirection = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

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

    private void FixedUpdate()
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

        //Lerp between the base an max speed, based on the timer value, move the root forward
        currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, speedUpDownTimer / accelTime);
        rb.MovePosition(rb.position + (Vector2)(-transform.up * currentSpeed) * Time.deltaTime);
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
    }
}
