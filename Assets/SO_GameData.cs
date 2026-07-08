using UnityEngine;
using Unity.Properties;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class SO_GameData : ScriptableObject
{
    [SerializeField, DontCreateProperty] float m_boostCurrent = 100;
    [SerializeField, DontCreateProperty] float m_boostMax = 100;


    //Variables to be saved, any new variables to be saved should be added here and to the DataPackage object in SaveData.cs
    [CreateProperty] public float totalWater;
    [CreateProperty] public float currentWater;
    [CreateProperty] public float previousLevelWater;

    //A lot of this stuff doesn't actually need to be saved yet, it's only used at runtime
    [CreateProperty] public float currentDistance;
    [CreateProperty] public float speed; 
    [CreateProperty] public float boostMax => m_boostMax;

    [CreateProperty] 
    public float CurrentBoost //genuinely no idea why we need both a getter/setter and public methods but it was in the tutorial
    {  
        get => Mathf.Clamp(m_boostCurrent, 0, m_boostMax);
        set
        {
            float clampedValue = Mathf.Clamp(value, 0, boostMax);
            m_boostCurrent = clampedValue;
        }
    }

    public void SubtractBoost(float subtract)//also apparently you can put methods into scriptable objects? i guess that's why they're scriptable?
    {
        CurrentBoost -= subtract;
    }

    public void AddBoost(float add)
    {
        CurrentBoost += add;
    }


    //Preformatted percentage to be used by UI but i don't even use it yet
    [CreateProperty] public float boostPercentage => CurrentBoost / m_boostMax;

    //Resets all data back to default
    public void ResetData()
    {
        totalWater = 0f;
        currentWater = 0f;
        previousLevelWater = 0f;
        currentDistance = 0f;
    }

    //Loads data from a DataPackage object (e.g. the local object used to send data to and from the scriptable object)
    public void LoadData(DataPackage package)
    {
        totalWater = package.totalWater;
        currentWater = package.currentWater;
        previousLevelWater = package.previousLevelWater;
        currentDistance = package.currentDistance;
    }
}
