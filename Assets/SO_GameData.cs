using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class SO_GameData : ScriptableObject
{
    //Variables to be saved, any new variables to be saved should be added here and to the DataPackage object in SaveData.cs
    public float totalWater;
    public float currentWater;
    public float previousLevelWater;

    //Resets all data back to default
    public void ResetData()
    {
        totalWater = 0f;
        currentWater = 0f;
        previousLevelWater = 0f;
    }

    //Loads data from a DataPackage object (e.g. the local object used to send data to and from the scriptable object)
    public void LoadData(DataPackage package)
    {
        totalWater = package.totalWater;
        currentWater = package.currentWater;
        previousLevelWater = package.previousLevelWater;
    }
}
