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
        if (!doneLaunching)
        {
            if (rotatingUp)
            {
                transform.Rotate(new Vector3(0, 0, 1) * rotateSpeed * Time.deltaTime);

                negativeEuler = transform.rotation.eulerAngles.z;

                if (negativeEuler >= 180)
                    negativeEuler -= 360;

                if (negativeEuler > maxAngle)
                {
                    transform.eulerAngles = new Vector3(0, 0, maxAngle);
                    rotatingUp = false;
                }
            }

            else
            {
                transform.Rotate(new Vector3(0, 0, 1) * -rotateSpeed * Time.deltaTime);

                negativeEuler = transform.rotation.eulerAngles.z;
                
                if (negativeEuler >= 180)
                    negativeEuler -= 360;

                if (negativeEuler < minAngle)
                {
                    transform.eulerAngles = new Vector3(0, 0, minAngle + 360);
                    rotatingUp = true;
                }
            }
        }
    }

    public Vector3 GetForward()
    {
        return transform.right;
    }

    public void StopRotating()
    {
        doneLaunching = true;
    }
}
