using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SeedControllerKin : MonoBehaviour
{

    private PlayerInput playerInput;
    private bool usingPropeller = false;
    private int rotationDirection = 0;
    private bool launched = false;
    private bool finished = false;

    private Rigidbody2D rb;

    public float rotationSpeed = 100;
    public float propellerStrength = 30;
    public float launchStrength = 1000;
    public Launcher seedLauncher;

    public float baseSlowdown = 5f;
    public float maxSlowdownIncrease = 10f;
    public float maxSpeedupIncrease = 5f;
    public float gravityValue = 9.8f;
    [Range(0, 90)]
    public float crashAngle = 60;

    private float forwardVelocityLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = seedLauncher.transform.position;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputPass();
    }

    void FixedUpdate()
    {
        //Only do movement updates once the seed is launched
        if (launched && !finished)
        {
            if (usingPropeller)
                rb.AddForce(transform.up * propellerStrength);

            //Rotate the seed around the Z axis according to the user's input
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x, 
                transform.eulerAngles.y, 
                transform.eulerAngles.z + rotationDirection * rotationSpeed * Time.deltaTime
                );

            Vector3 totalVelocity = Vector2.zero;

            //Grab the Y component of the seed's right vector
            float rightY = transform.right.y;

            //if the Y component is >0 (e.g. the seed is angled upwards)
            if (rightY > 0)
            {
                //Subract from the total forward velocity. maxSlowdownIncrease is the maximum amount we want to be subtracted per frame,
                //so lerp towards that at a rate corresponding to how intensely the seed is angeled upwards
                forwardVelocityLength -= Mathf.Lerp(0f, maxSlowdownIncrease, rightY) * Time.deltaTime;
            }

            //if the Y component is <0 (e.g. the seed is angled downwards)
            else if (rightY < 0)
            {
                //Add to the total forward velocity. maxSpeedupIncrease is the maximum amount we want to add per frame,
                //so lerp towards that at a rate corresponding to how intensely the seed is angeled downwards
                forwardVelocityLength += Mathf.Lerp(0f, maxSpeedupIncrease, Mathf.Abs(rightY)) * Time.deltaTime;
            }

            //Always subtract some air drag
            forwardVelocityLength -= baseSlowdown * Time.deltaTime;

            //Always ensure that the velocity is >0, otherwise the seed is unable to speed up again at all
            forwardVelocityLength = Mathf.Max(0.2f, forwardVelocityLength);

            //Add the forward velocity and gravity (downward) velocity
            totalVelocity += forwardVelocityLength * transform.right;
            totalVelocity += gravityValue * Vector3.down;

            rb.MovePosition(rb.position + new Vector2(totalVelocity.x, totalVelocity.y) * Time.deltaTime);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            float currentAngle = transform.eulerAngles.z;

            //Convert to +/- 180 degree range instead of 0-360 degree range
            if (currentAngle > 180)
                currentAngle -= 360;

            //Find the difference between 90 degrees and the pre-determined crash angle.
            //crashDifference will be used to ensure that the player isn't crashing while facing backwards.
            float crashDifference = 90 - crashAngle;

            //Check to see if the player is exceeding the crash angle from either the forward or backwards direction.
            //Logic: the current angle needs to be exceeding the crash angle, but no greater than the crash angle reflected (e.g. going the other way)
            //For example: given a crashAngle of 80, if the player is moving forward, then they would crash anywhere from 80-90 degress.
            //If the player is moving backwards, they would crash anywhere from 90-100 degrees.
            if (Mathf.Abs(currentAngle) >= crashAngle && Mathf.Abs(currentAngle) <= 90 + crashDifference)
            {
                HandleCrash();
            }
            /*
            else
            {
                Vector2 collisionNormal = collision.contacts[0].normal;

                transform.position += (Vector3)(collisionNormal * 0.5f);
                //rb.MovePosition(rb.position + new Vector2(0, 10));

                //Vector3 newAngle = Vector2.Reflect(transform.eulerAngles, collision.contacts[0].normal);

                //Debug.Log(transform.eulerAngles);
                //Debug.Log(newAngle);

                //transform.eulerAngles = newAngle;

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -transform.eulerAngles.z);
            }
            */
        }
    }

    private void InputPass()
    {

        if (playerInput.actions["Launch_Seed"].WasPressedThisFrame())
        {
            LaunchSeed();
        }

        //Once launched, we can check input for other seed movement
        if (launched)
        {
            //Check for using propeller, either true or false
            if (playerInput.actions["Use_Propeller"].WasPressedThisFrame())
                usingPropeller = true;

            else if (playerInput.actions["Use_Propeller"].WasReleasedThisFrame())
                usingPropeller = false;

            //Check for rotation, +1 for clockwise, -1 for counterclockwise.

            if (playerInput.actions["Tilt_Clockwise"].WasPressedThisFrame())
                rotationDirection--;

            if (playerInput.actions["Tilt_Counterclockwise"].WasPressedThisFrame())
                rotationDirection++;

            if (playerInput.actions["Tilt_Clockwise"].WasReleasedThisFrame())
                rotationDirection++;

            if (playerInput.actions["Tilt_Counterclockwise"].WasReleasedThisFrame())
                rotationDirection--;
        }
    }

    //Logic for launching the seed
    private void LaunchSeed()
    {
        //Only do the launch logic if it's not already launched
        if (!launched)
        {
            //Set launched to true, stop rotating the launcher
            launched = true;
            seedLauncher.StopRotating();

            //Set the seed's rotation equal to the launcher's and set the initial velocity
            transform.rotation = seedLauncher.transform.rotation;
            forwardVelocityLength = launchStrength * 10;
        }
    }

    private void HandleCrash()
    {
        forwardVelocityLength = 0f;
        finished = true;
    }
}

