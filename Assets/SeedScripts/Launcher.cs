using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    public float minAngle = -85;
    public float maxAngle = 85;
    public float rotateSpeed = 100;

    bool rotatingUp = true;
    bool doneLaunching = false;

    private PlayerInput playerInput;
    private float negativeEuler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        //Only do rotation logic if the launcher hasn't been activated
        if (!doneLaunching)
        {
            //Rotating up
            if (rotatingUp)
            {
                //Rotate upwards at the specified speed
                transform.Rotate(new Vector3(0, 0, 1) * rotateSpeed * Time.deltaTime);
                
                //Find the euler Z value
                negativeEuler = transform.rotation.eulerAngles.z;

                //Convert the euler value to 180-degree range, easier to work with
                if (negativeEuler > 180)
                    negativeEuler -= 360;

                //If the rotation reaches the max angle, switch to rotating downwards
                if (negativeEuler > maxAngle)
                {
                    transform.eulerAngles = new Vector3(0, 0, maxAngle);
                    rotatingUp = false;
                }
            }

            else
            {
                //Rotate downwards
                transform.Rotate(new Vector3(0, 0, 1) * -rotateSpeed * Time.deltaTime);

                //Find the Z euler value
                negativeEuler = transform.rotation.eulerAngles.z;
                
                //Convert to 180-degree range
                if (negativeEuler > 180)
                    negativeEuler -= 360;

                //If we reach the minimum angle, start rotating upwards
                if (negativeEuler < minAngle)
                {
                    transform.eulerAngles = new Vector3(0, 0, minAngle + 360);
                    rotatingUp = true;
                }
            }
        }
    }

    //Function to use to get the forward vector, useful for launching the seed (I don't think I even use this lol)
    public Vector3 GetForward()
    {
        return transform.right;
    }

    //Function to tell the launcher to stop rotating, typically called when the seed is launched
    public void StopRotating()
    {
        doneLaunching = true;
    }
}
