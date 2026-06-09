using UnityEngine;
using UnityEngine.Splines;

public class ObstacleValues : MonoBehaviour
{
    [Range(0, .999f)] public float slowdownRate = 0.25f;
    public float minScale = 1f;
    public float maxScale = 1f;
    public int spawnChance = 10;
    public int breakStrength = 5;

    [System.NonSerialized]
    public float boundingRadius;

    /*
    [Header("Movement details. No need to touch these values if the obstacle doesn't move.")]
    public bool obstacleMoves = false;
    public float moveSpeed = 10f;
    public SplineContainer movementPath;
    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
