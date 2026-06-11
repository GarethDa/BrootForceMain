using UnityEngine;

public class InteractiveType : MonoBehaviour
{
    public float minScale = 1f;
    public float maxScale = 1f;
    public int spawnChance = 10;
    public bool randomRotation = true;

    [System.NonSerialized]
    public float boundingRadius;

    public void Deactivate()
    {
        GetComponent<SpriteRenderer>().color *= 0.5f;
        Destroy(GetComponent<Collider2D>());
    }

}
