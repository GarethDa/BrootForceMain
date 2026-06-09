using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    public List<BiomeType> biomes;
    public BiomeTypes biome;

    private BiomeType currentBiome;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Grab the correct biome from the list of biome types, based on the biome type selected from the
        //BiomeTypes enum.
        foreach (BiomeType thisBiome in biomes)
        {
            if (thisBiome.name.ToLower().Equals(biome.ToString()))
            {
                currentBiome = thisBiome;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetBiomeBackground()
    {
        return currentBiome.repeatableTexture;
    }

    public float GetBiomeSlowdown()
    {
        return currentBiome.slowdownPercentage;
    }

    public BiomeType GetCurrentBiome()
    {
        return currentBiome;
    }

    public enum BiomeTypes
    {
        dirt,
        sand,
        stone
    }
}
