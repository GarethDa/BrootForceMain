using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SeedController : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool usingPropeller = false;
    private int rotationDirection = 0;
    private bool launched = false;
    private bool grounded = false;
    private float currentFuel;
    private Rigidbody2D rb;

    [Header("Basic Movement Variables")]
    public float rotationSpeed = 100;
    public Launcher seedLauncher;
    public float launchStrength = 1000;
    public float diveBoost = 5f;
    public float gravityScale = 0.1f;
    [Range(.1f, 90f)]
    public float crashAngle = 80f;

    [Header("Booster Variables")]
    public float thrustStrength = 10f;
    public float maxFuel = 100f;
    public float fuelBurnRate = 5f;

    [Header("Drag Variables")]
    public float baseDragCoefficient = 0.05f;
    public float maxDragCoefficientBoost = 0.5f;
    public float airDensity = 1.225f;
    public float surfaceArea = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = seedLauncher.transform.position; //Initialize with the seed sitting inside the launcher
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //Gravity scale starts at 0, change it when the seed is launched
        currentFuel = maxFuel;
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
            if (!grounded)
            {
                //Rotate the seed corresponding to the player's input
                rb.MoveRotation(rb.rotation + rotationDirection * rotationSpeed * Time.deltaTime);

                //Calculate the angle of attack.This is defined as the angle difference between the glider's current orientation
                //and the glider's actual velocity. When the player changes the direction of the glider, the linear velocity isn't going to 
                //perfectly align with this change in direction immediately, and this is what the angle of attack captures.
                float angleOfAttack = Mathf.Deg2Rad * Vector2.SignedAngle(transform.right, rb.linearVelocity);

                //If the seed is angled below its current velocity
                if (transform.right.y < rb.linearVelocity.normalized.y)
                {
                    //Calculate the dive intensity. This sits between 0 and 1, where 0 means the seed is perfectly aligned with
                    //the velocity, and 1 means the seed is facing 90 degrees perpendicular from the velocity.
                    float diveIntensity = Mathf.Clamp01(Mathf.Abs(angleOfAttack) * (2 / Mathf.PI));

                    //Calculate the total boost in the direction that the seed is facing, using the diveBoost variable, and apply the force.
                    Vector2 forwardBoost = transform.right * (diveBoost * diveIntensity);
                    rb.AddForce(forwardBoost);
                }

                //Safeguard: If the seed is facing above its velocity, ensure that the angle of attack is positive. Otherwise, ensure it's negative.
                if (transform.right.y > rb.linearVelocity.normalized.y)
                    angleOfAttack = Mathf.Abs(angleOfAttack);

                else
                    angleOfAttack = -Mathf.Abs(angleOfAttack);

                //Calculate the lift coefficient. Using 2 * sin, this means that the lift peaks at 45 degrees, and reduces going above or below this.
                float liftCoefficient = Mathf.Sin(2 * angleOfAttack);
                //float liftCoefficient = 2 * Mathf.PI * angleOfAttack; //old method, broke on higher angles.
                
                //Clamp the coefficient at a certain value, this might need to be adjusted later.
                liftCoefficient = Mathf.Clamp(liftCoefficient, -1.5f, 1.5f);
                
                //The direction of lift should always be perpendicular to the velocity of the flying object.
                Vector2 liftDir = Vector2.Perpendicular(rb.linearVelocity).normalized;
                //Invert the lift direction if the seed's up direction is opposite of it. This should mainly happen when the seed is upside down.
                if (Vector2.Dot(liftDir, transform.up) < 0f)
                    liftDir *= -1;

                //Calculate the total lift force. This is using the following basic lift calculation from NASA:
                //https://www1.grc.nasa.gov/beginners-guide-to-aeronautics/lift-equation/
                Vector2 liftForce = liftCoefficient * (airDensity * rb.linearVelocity.sqrMagnitude * surfaceArea * 0.5f) * liftDir;
                rb.AddForce(liftForce);

                //Calculate a dynamic drag coefficient which is at minimum the base, and is at maximum the base + the boost.
                //The coefficient maxes out when the angle of attack is 90 degrees (e.g. the seed is perpendicular to the velocity), which should cause large drag.
                float currentDragCoefficient = baseDragCoefficient + (maxDragCoefficientBoost * Mathf.Pow(Mathf.Sin(angleOfAttack), 2));
                //Vector2 dragForce = dragCoefficient * (airDensity * Mathf.Pow(rb.linearVelocity.magnitude, 2) * surfaceArea * 0.5f) * -rb.linearVelocity.normalized;

                //Calculate the drag force and apply it. This uses a combination of NASA's drag equation and an implicit velocity system which prevents drag from overshooting.
                //https://www1.grc.nasa.gov/beginners-guide-to-aeronautics/falling-object-with-air-resistance/
                //https://discussions.unity.com/t/physics-drag-formula/541279/9
                float dragFactor = currentDragCoefficient * airDensity * rb.linearVelocity.magnitude * surfaceArea * 0.5f;
                if (rb.linearVelocity.magnitude > 0.1f)
                    rb.linearVelocity = rb.linearVelocity / (1f + (dragFactor * Time.deltaTime));

                //If we're using the booster
                if (usingPropeller)
                {
                    //Calculate the thrust force based on the determined thrust strength, subtract from the current fuel
                    Vector2 thrustForce = transform.right * thrustStrength;
                    currentFuel -= fuelBurnRate * Time.deltaTime;

                    //If we still have fuel, apply the thrust force
                    if (currentFuel > 0)
                        rb.AddForce(thrustForce);
                    
                    Debug.Log("Fuel left: " + currentFuel);
                }

            }

            else
            {
                //******Level end logic here!!
            }
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
            rb.gravityScale = gravityScale; //Set the gravity scale
            seedLauncher.StopRotating();
            transform.rotation = seedLauncher.transform.rotation; //Align the seed's rotation with the launcher
            rb.AddForce(launchStrength * transform.right, ForceMode2D.Impulse); //Apply the initial launch impulse
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;

            //Find the angle of collision. If it exceeds the specified crash angle, then immediately end the seed's movement.
            float collisionAngle = 90 - Vector2.Angle(transform.right, -collision.contacts[0].normal);
            if (collisionAngle >= crashAngle)
                rb.linearVelocity = Vector2.zero;
        }
    }
}

