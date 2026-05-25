using UnityEngine;

public class CamControls : MonoBehaviour
{
    public GameObject rootBase;
    public float verticalOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rootBase.transform.position + new Vector3(0f, -verticalOffset, transform.position.z);
    }
}
