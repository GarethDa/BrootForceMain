using UnityEngine;
using UnityEngine.Splines;

public class MovingObstacleType : ObstacleType
{
    public float moveSpeed = 100f;
    public SplineContainer movementPath;
    public bool stretchToBoundaries = false;

    private SplineAnimate animateComponent;
    private SplineContainer thisPath;

    private void Awake()
    {
        animateComponent = GetComponent<SplineAnimate>();
        animateComponent.MaxSpeed = moveSpeed;
        Debug.Log("spline component set");
    }

    public void SetupPath(Vector2 pos)
    {
        Debug.Log("spline component being used");
        thisPath = Instantiate(movementPath, pos, Quaternion.identity);
        thisPath.transform.parent = GameObject.Find("SpawnedObjects").transform;
        animateComponent.Container = thisPath;
    }

    public override void DestroyObject()
    {
        base.DestroyObject();
        Destroy(thisPath);
    }

    public void StartMoving()
    {
        animateComponent.Play();
    }

    public void StopMoving()
    {
        animateComponent.Pause();
    }
}
