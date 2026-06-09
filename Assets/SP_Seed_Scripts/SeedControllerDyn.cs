using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SeedController : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool usingPropeller = false;
    private int rotationDirection = 0;
    private bool launched = false;

    private Rigidbody2D rb;

    public float rotationSpeed = 100;
    public float propellerStrength = 30;
    public float launchStrength = 1000;
    public Launcher seedLauncher;

    public float standardDamping = 5f;
    public float maxDamping = 30f;
    public float minDamping = 1f;
    public float gravityScale = 0.1f;
    public float maxGravityScale = 5f;
    public float flightSpeed = 10f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        transform.position = seedLauncher.transform.position;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        //rb.linearDamping = standardDamping;
    }

    // Update is called once per frame
    void Update()
    {
        InputPass();
    }

    void FixedUpdate()
    {
        if (launched)
        {
            if (usingPropeller)
                rb.AddForce(transform.up * propellerStrength);
            //rb.AddForce(new Vector2(0f, 50f));

            rb.angularVelocity = rotationDirection * rotationSpeed;

            rb.AddForce(transform.right * flightSpeed);

            float rightY = transform.right.y;

            if (rightY > 0)
            {
                float forwardSpeed = rb.linearVelocity.x;
                float horizontalSlowdown = forwardSpeed * rightY * maxDamping;

                rb.AddForce(Vector2.left * horizontalSlowdown);
            }

            else
            {
            }

            //float currentDrag = transform.eulerAngles.z;

            //if (currentDrag >= 180)
            //    currentDrag -= 360;

            //currentDrag *= dragPerDegree;

            //rb.linearDamping = standardDamping;

            //Debug.Log(currentDrag);
            //float forwardSpeed = Vector2.Dot(rb.linearVelocity, transform.right);
            //rb.AddForce(transform.up * (forwardSpeed * liftCoefficient));
        }
    }

    private void InputPass()
    {

        if (playerInput.actions["Launch_Seed"].WasPressedThisFrame())
        {
            Debug.Log("here");
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

    private void LaunchSeed()
    {
        if (!launched)
        {
            launched = true;
            rb.gravityScale = gravityScale;
            seedLauncher.StopRotating();
            transform.rotation = seedLauncher.transform.rotation;

            //rb.linearVelocity = seedLauncher.GetForward() * launchStrength;

            //rb.linearVelocity = 100 * transform.right;
            //rb.AddForce(transform.right * launchStrength);
        }
    }
}
