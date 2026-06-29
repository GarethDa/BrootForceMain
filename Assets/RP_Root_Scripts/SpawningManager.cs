using UnityEngine;
using System.Collections.Generic;

public class SpawningManager : MonoBehaviour
{
    public List<GameObject> interactiveObjects;
    public GameObject spawningWidth;
    public GameObject backgroundObject;
    public Camera playerCam;
    public int numSpawnRowsColumns = 10;
    public int minObstaclesPerChunk = 10;
    public int maxObstaclesPerChunk = 25;
    public GameObject obstacleHolder;
    public GameObject barrierObject;

    private float currentMinY = 0;
    private float currentChunkNum = 0;
    private float chunkHeight;
    private int totalSpawnChance = 0;
    private SpriteRenderer backgroundRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //We're making a square chunk, so set the height equal to the width.
        chunkHeight = spawningWidth.transform.localScale.x;

        //Get the renderer component, set the sprite to be the level manager's selected background texture,
        //and set the size equal to a square defined by the chunk height (which is derived from the provided spawningWidth object)
        backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
        backgroundRenderer.sprite = LevelManager.instance.GetBiomeBackground();
        backgroundRenderer.size = new Vector2(chunkHeight, chunkHeight);

        //Add each spawnable object type's spawn chance to the total spawn chance
        foreach (GameObject gObject in interactiveObjects)
            totalSpawnChance += gObject.GetComponent<InteractiveType>().spawnChance;
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
            foreach (GameObject gObject in interactiveObjects)
            {
                currentSpawnValue += gObject.GetComponent<InteractiveType>().spawnChance;

                if (currentSpawnValue >= spawnNumber)
                {
                    objectsToSpawn.Add(gObject);
                    break;
                }
            }
        }

        //*****Logic for spawning the selected obstacles
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
                    -spawningWidth.transform.localScale.x / 2,
                    spawningWidth.transform.localScale.x / 2);

                float spawnY = Random.Range(
                    -chunkHeight * currentChunkNum,
                    -chunkHeight * (currentChunkNum + 1));

                Vector2 spawnPos = new Vector2(spawnX, spawnY);

                PolygonCollider2D objCollider = gObject.GetComponent<PolygonCollider2D>();

                //Get a random object scale using the provided values from the InteractiveType component
                float objScale = Random.Range(gObject.GetComponent<InteractiveType>().minScale, gObject.GetComponent<InteractiveType>().maxScale);

                //Calculate the radius of the bounding circle, to be used for avoiding overlapping objects
                float approxRadius = Vector2.Distance(objCollider.bounds.center, objCollider.bounds.max) * objScale;

                bool spawnIt = true;

                //Look through each object that's already been spawned in this chunk (starts at zero)
                foreach (GameObject spawnedObject in spawnedObstacles)
                {

                    //If the new obstacle's position overlaps with this existing obstacle, then break out of this for loop
                    //and set spawnIt to false, which indicates that a new position should be generated
                    if (Vector2.Distance(spawnedObject.transform.position, spawnPos) <
                        spawnedObject.GetComponent<InteractiveType>().boundingRadius + approxRadius)
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
                    
                    //Set the bounding radius. This makes it very simple to grab this radius for future overlap calculations
                    //instead of having to re-calculate it every time
                    newObject.GetComponent<InteractiveType>().boundingRadius = approxRadius;

                    //Set the scale and randomize the rotation
                    newObject.transform.localScale = new Vector3(objScale, objScale, 1);

                    //If we're working with a collectible here, set its value based on the scale
                    //(if the collectible isn't set to have its value align with scale, then SetThisValue just sets
                    //the value to the pre-selected value without multiplying by the scale.
                    if (newObject.TryGetComponent(out CollectibleType colType))
                        colType.SetThisValue(objScale);

                    //If the object has random rotation selected, then randomly rotate it
                    if (newObject.GetComponent<InteractiveType>().randomRotation)
                        newObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));

                    //Add the obstacle to the list of spawned obstacles for this chunk
                    spawnedObstacles.Add(newObject);
                    break;
                }

                attempts++;
            }
        }

        float barrierWidth = barrierObject.GetComponent<SpriteRenderer>().size.x;

        //Create a new background for the chunk
        GameObject newBackground = Instantiate(
            backgroundObject, 
            new Vector3(0f, -chunkHeight * currentChunkNum - (chunkHeight / 2)), 
            Quaternion.identity, 
            obstacleHolder.transform);

        //Create a new left and right barrier for the chunk
        GameObject newLeftBarrier = Instantiate(
            barrierObject, 
            new Vector3((-chunkHeight * 0.5f) - (barrierWidth * 0.5f), -chunkHeight * currentChunkNum - (chunkHeight / 2)), 
            Quaternion.identity, 
            obstacleHolder.transform);

        GameObject newRightBarrier = Instantiate(
            barrierObject,
            new Vector3((chunkHeight * 0.5f) + (barrierWidth * 0.5f), -chunkHeight * currentChunkNum - (chunkHeight / 2)),
            Quaternion.identity,
            obstacleHolder.transform);

        newLeftBarrier.GetComponent<SpriteRenderer>().size = new Vector2(barrierWidth, chunkHeight);
        newRightBarrier.GetComponent<SpriteRenderer>().size = new Vector2(barrierWidth, chunkHeight);
    }
}
