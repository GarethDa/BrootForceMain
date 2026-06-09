using UnityEngine;

[CreateAssetMenu(fileName = "BiomeType", menuName = "Scriptable Objects/BiomeType")]
public class BiomeType : ScriptableObject
{
    public string biomeName;
    public Sprite repeatableTexture;
    [Range(0, .99f)]
    public float slowdownPercentage;
}
