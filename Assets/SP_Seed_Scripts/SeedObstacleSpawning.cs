using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SeedObstacleSpawning : MonoBehaviour
{
    [SerializeField] GameObject testSpawnEntity;
    [SerializeField] GameObject playerRef;
    private Camera mainCam;
    [SerializeField] List<GameObject> activeObstacles = new List<GameObject>();
    [SerializeField] float spawnTimer = 5f;
    float timeSinceLastSpawn = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastSpawn += Time.fixedDeltaTime;

        Vector2 viewEdge = mainCam.ViewportToWorldPoint(new Vector3(1.1f, 0.5f)); //gets the right edge of the camera's view in worldspace

        if (timeSinceLastSpawn > spawnTimer)
        {
            Vector2 randomDispersion = Random.insideUnitCircle * Random.Range(0f, 10f);

            Vector3 spawnPoint = viewEdge + randomDispersion;

            Instantiate(testSpawnEntity, spawnPoint, Quaternion.identity);
            Debug.Log("Spawn point: " + spawnPoint + "   Random: " + randomDispersion + "   View edge: " + viewEdge);

            timeSinceLastSpawn = 0f;
        }
    }
}
