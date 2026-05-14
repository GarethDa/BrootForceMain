using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    public Vector2 cameraOffset = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(cameraOffset.x, cameraOffset.y, transform.position.z);
    }
}
