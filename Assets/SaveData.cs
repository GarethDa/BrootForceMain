using UnityEngine;

public class SaveData : MonoBehaviour
{
    public GameData gameData = new GameData();

    private void OnEnable()
    {
        //Subscribe the relevant methods to their corresponding events
        RootMovement.OnWaterCollected += UpdateWater;
        LevelManager.OnLevelEnded += EndLevel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFromJson();
        }
    }

    public void SaveGame()
    {
        //Convert the game data to a json and get the dedicated savegame file path
        string saveInfo = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + "/GameData.json";

        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, saveInfo);
        Debug.Log("Saved");
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/GameData.json";
        string saveInfo = System.IO.File.ReadAllText(filePath);

        gameData = JsonUtility.FromJson<GameData>(saveInfo);
        Debug.Log("Loaded");
    }

    private void UpdateWater(float addedWater)
    {
        gameData.currentWater += addedWater;
    }

    //This should be called when the root level ends, in order to save the current water values
    private void EndLevel()
    {
        gameData.previousLevelWater = gameData.currentWater;
        gameData.totalWater += gameData.currentWater;
        gameData.currentWater = 0f;
    }
}

//This is the basic GameData object type. Every time we add a new variable/currency/etc. we can add it to this object type,
//resulting in it being saved (still have to send the data to this script though, usually through an event call)
[System.Serializable]
public class GameData
{
    public float totalWater;
    public float currentWater;
    public float previousLevelWater;
}
