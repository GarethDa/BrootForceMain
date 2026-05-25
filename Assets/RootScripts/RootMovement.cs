using UnityEngine;
using UnityEngine.InputSystem;

public class RootMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 10f;

    private int turnDirection = 0;
    private PlayerInput playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        InputPass();
    }

    private void FixedUpdate()
    {
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


    }
}
