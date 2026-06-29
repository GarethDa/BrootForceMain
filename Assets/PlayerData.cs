using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int waterLevel;
    public int distance;
    public int launches;
    public float maxBoost;
}
