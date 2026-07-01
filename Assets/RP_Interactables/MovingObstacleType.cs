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
    }

    public void SetupPath(Vector2 pos)
    {
        thisPath = Instantiate(movementPath, pos, Quaternion.identity);
        animateComponent.Container = thisPath;
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
