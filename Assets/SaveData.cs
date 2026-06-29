using UnityEngine;

public class SaveData : MonoBehaviour
{
    public SO_GameData gameData;

    private void OnEnable()
    {
        //Subscribe the relevant methods to their corresponding events
        RootMovement.OnWaterCollected += UpdateWater;
        LevelManager.OnLevelEnded += EndLevel;
        gameData.ResetData();
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
        DataPackage newPackage = new DataPackage(gameData);

        //Convert the game data to a json and get the dedicated savegame file path
        string saveInfo = JsonUtility.ToJson(newPackage, true);
        string filePath = Application.persistentDataPath + "/GameData.json";

        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, saveInfo);
        Debug.Log("Saved");
    }

    public void LoadFromJson()
    {
        //Specify the file path and read the JSON located there
        string filePath = Application.persistentDataPath + "/GameData.json";
        string saveInfo = System.IO.File.ReadAllText(filePath);

        DataPackage loadedPackage = JsonUtility.FromJson<DataPackage>(saveInfo);

        //Assign the variables from the loaded package into the scriptable object
        gameData.LoadData(loadedPackage);

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

//This is a local DataPackage object to be used for sending data between the savegame files and the scriptable object data holder.
//Every time a new variable/object/etc. type needs to be tracked for save game data, it should be added here and to the scriptable object (e.g. SO_GameData).
[System.Serializable]
public class DataPackage
{
    public float totalWater;
    public float currentWater;
    public float previousLevelWater;

    //On construction, the package object should pull all of the information from the scriptable object.
    public DataPackage(SO_GameData sourceGameData)
    {
        totalWater = sourceGameData.totalWater;
        currentWater = sourceGameData.currentWater;
        previousLevelWater = sourceGameData.previousLevelWater;
    }
}
