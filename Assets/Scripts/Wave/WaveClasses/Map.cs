using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public int levelNumber = 0;
    public float spawnRate = 1f;
    public int waveCount = 3;
    public int spawnPerWave = 100;
    public int decoyProbability = 3;
    public SerializableLevel loadLevel;

    public void SetLookup()
    {
        LevelLookup.spawnRate = spawnRate;
        LevelLookup.waveCount = waveCount;
        LevelLookup.spawnPerWave = spawnPerWave;
        LevelLookup.decoyProbability = decoyProbability;
    }

}
