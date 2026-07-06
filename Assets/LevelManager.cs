using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }
    public static event Action OnLevelEnded;

    [Header("Biome spawning variables")]
    public List<BiomeType> biomes;
    public BiomeTypes biome;

    [Header("Timer UI variables (this will be changed when we work on UI)")]
    public TMP_Text timerText;
    public float levelDuration = 20f;

    //public float timerUpdateTime = 0.5f;
    private string timerTextPreamble;

    private float currentTimer;

    private BiomeType currentBiome;

    private bool levelEnded = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerTextPreamble = timerText.text;
        currentTimer = levelDuration;

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
        currentTimer -= Time.deltaTime;

        timerText.text = timerTextPreamble + Mathf.Ceil(currentTimer);

        if (currentTimer <= 0 && !levelEnded)
        {
            levelEnded = true;
            OnLevelEnded?.Invoke();
            Time.timeScale = 0f;
            //****Do level switching here for now!!
        }

    }

    public Sprite GetBiomeBackground()
    { return currentBiome.repeatableTexture; }

    public float GetBiomeSlowdown()
    { return currentBiome.slowdownPercentage; }

    public BiomeType GetCurrentBiome()
    { return currentBiome; }

    public enum BiomeTypes
    {
        dirt,
        sand,
        stone
    }
}
