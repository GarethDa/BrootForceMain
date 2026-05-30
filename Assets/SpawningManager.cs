using UnityEngine;
using System.Collections.Generic;

public class SpawningManager : MonoBehaviour
{
    public List<GameObject> obstacleObjects;
    public List<Sprite> backgrounds;
    public BiomeTypes biome;
    public GameObject spawningWidth;
    public GameObject backgroundObject;
    public Camera playerCam;
    public int numSpawnRowsColumns = 10;
    public int minObstaclesPerChunk = 10;
    public int maxObstaclesPerChunk = 25;
    public GameObject obstacleHolder;

    private float currentMinY = 0;
    private float currentChunkNum = 0;
    private float chunkHeight;
    private int totalSpawnChance = 0;
    private Sprite currentBackground;
    private SpriteRenderer backgroundRenderer;


    //private float camHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //We're making a square chunk, so set the height equal to the width.
        chunkHeight = spawningWidth.transform.localScale.x;

        foreach (Sprite tex in backgrounds)
        {
            if (tex.name.Equals(biome.ToString()))
            {
                currentBackground = tex;
                break;
            }
        }

        backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
        backgroundRenderer.sprite = currentBackground;
        backgroundRenderer.size = new Vector2(chunkHeight, chunkHeight);

        //Add each spawnable object type's spawn chance to the total spawn chance
        foreach (GameObject gObject in obstacleObjects)
            totalSpawnChance += gObject.GetComponent<ObstacleValues>().spawnChance;

        //backgroundObject.transform.position = new Vector3(0f, chunkHeight / 2, 0f);

        //This logic is all unnecessary right now, might be needed if I change the algorithm though
        /*
        //Look through each obstacle, find the largest possible length of an obstacle.
        //Logic: the largest possible obstacle will have either the longest width or height, multiplied by the maximum scale
        //as defined by its ObstacleValues component.
        foreach (GameObject gObject in obstacleObjects)
        {
            float largestScale = gObject.GetComponent<ObstacleValues>().maxScale;

            if (gObject.GetComponent<PolygonCollider2D>().bounds.size.x * largestScale > biggestLength)
                biggestLength = gObject.GetComponent<PolygonCollider2D>().bounds.size.x * largestScale;

            if (gObject.GetComponent<PolygonCollider2D>().bounds.size.y * largestScale > biggestLength)
                biggestLength = gObject.GetComponent<PolygonCollider2D>().bounds.size.y * largestScale;

            totalSpawnChance += gObject.GetComponent<ObstacleValues>().spawnChance;
        }

        //For the sake of the spawning algorithm, the simplest way to guarantee that no objects overlap is to assume that
        //there is a scenario where tow copies of the largest obstacle possible are going to be spawned side-by-side.
        biggestLength *= 2;
        */
    }

    // Update is called once per frame
    void Update()
    {
        //Update the current lowest depth reach whenever a new lowest depth is reached
        if (currentMinY > playerCam.transform.position.y - playerCam.orthographicSize)
            currentMinY = playerCam.transform.position.y - playerCam.orthographicSize;

        //Once the root gets close enough to the new chunk, populate the chunk with random spawns (e.g. SpawnChunk())
        if (currentMinY <= (-chunkHeight * currentChunkNum) + (playerCam.orthographicSize * 2))
        {
            SpawnChunk();
            currentChunkNum++;
        }
    }

    void SpawnChunk()
    {
        //*****Logic for selecting the obstacles to spawn

        //Select the number of obstacles for this chunk, between the min and max defined.
        int numObstacles = Random.Range(minObstaclesPerChunk, maxObstaclesPerChunk);

        Debug.Log(numObstacles);

        //Make a list to hold the objects to spawn
        List<GameObject> objectsToSpawn = new List<GameObject>();

        //Loop for finding the obstacles
        for (int i = 0; i < numObstacles; i++)
        {
            //Pick a random spawn number. This will be used to determine which object spawns
            int spawnNumber = Random.Range(1, totalSpawnChance);

            int currentSpawnValue = 0;

            /*
            Go through the list of possible obstacle types. For each obstacle type:
            - Add the obstacle's spawn chance to the current spawn value
            - If this addition brings the spawn value up to or over the randomly selected spawn number,
            then we have found the obstacle to spawn
            - Either add this obstacle to the list of objects to spawn and break the for loop,
            or continue on to the next obstacle type.
            */
            foreach (GameObject gObject in obstacleObjects)
            {
                currentSpawnValue += gObject.GetComponent<ObstacleValues>().spawnChance;

                if (currentSpawnValue >= spawnNumber)
                {
                    objectsToSpawn.Add(gObject);
                    Debug.Log(gObject.name);
                    break;
                }
            }
        }

        //*****Logic for spawning the select obstacles
        List<GameObject> spawnedObstacles = new List<GameObject>();

        //Look through each selected object type
        foreach (GameObject gObject in objectsToSpawn)
        {
            //Start attempts at zero, try to spawn this object a certain number of times
            int attempts = 0;
            while (attempts < 30)
            {
                //Select a random spawn range which falls within the chunk.
                float spawnX = Random.Range(
                    spawningWidth.transform.position.x - spawningWidth.transform.localScale.x / 2,
                    spawningWidth.transform.position.x + spawningWidth.transform.localScale.x / 2);

                float spawnY = Random.Range(
                    -chunkHeight * currentChunkNum,
                    -chunkHeight * (currentChunkNum + 1));

                Vector2 spawnPos = new Vector2(spawnX, spawnY);

                PolygonCollider2D objCollider = gObject.GetComponent<PolygonCollider2D>();

                //Get a random object scale using the provided values from the ObstacleValues component
                float objScale = Random.Range(gObject.GetComponent<ObstacleValues>().minScale, gObject.GetComponent<ObstacleValues>().maxScale);

                //Calculate the radius of the bounding circle, to be used for avoiding overlapping objects
                float approxRadius = Vector2.Distance(objCollider.bounds.center, objCollider.bounds.max) * objScale;

                bool spawnIt = true;

                //Look through each object that's already been spawned in this chunk (starts at zero)
                foreach (GameObject spawnedObject in spawnedObstacles)
                {

                    //If the new obstacle's position overlaps with this existing obstacle, then break out of this for loop
                    //and set spawnIt to false, which indicates that a new position should be generated
                    if (Vector2.Distance(spawnedObject.transform.position, spawnPos) <
                        spawnedObject.GetComponent<ObstacleValues>().boundingRadius + approxRadius)
                    {
                        spawnIt = false;
                        break;
                    }
                }

                //If we went through all of the existing obstacles and there is no overlap, then we can spawn the new obstacle
                if (spawnIt)
                {
                    //Instantiate the obstacle at the randomized position, make it a child of the obstacle holder
                    GameObject newObject = Instantiate(gObject, spawnPos, Quaternion.identity, obstacleHolder.transform);
                    
                    //Set the bounding radius. This makes it very simple to grab his radius for future overlap calculations
                    //instead of having to re-calculate it every time
                    newObject.GetComponent<ObstacleValues>().boundingRadius = approxRadius;

                    //Set the scale and randomize the rotation
                    newObject.transform.localScale = new Vector3(objScale, objScale, 1);
                    newObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));

                    //Add the obstacle to the list of spawned obstacles for this chunk
                    spawnedObstacles.Add(newObject);
                    break;
                }

                attempts++;
            }
        }

        GameObject newBackground = Instantiate(backgroundObject, new Vector3(0f, -chunkHeight * currentChunkNum - (chunkHeight / 2)), Quaternion.identity, obstacleHolder.transform);
    }

    public enum BiomeTypes
    {
        dirt,
        sand,
        stone
    }
}
