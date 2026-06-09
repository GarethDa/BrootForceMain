using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    public Vector2 cameraOffset = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        //Move the camera to the specified offset position
        transform.position = player.transform.position + new Vector3(cameraOffset.x, cameraOffset.y, transform.position.z);
    }
}
