using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefab for a Map which can be loaded
/// </summary>
/// <seealso cref="LevelLookup"/>
public class Map : MonoBehaviour {

    public int levelNumber = 0;
    public float spawnRate = 1f;
    public int waveCount = 3;
    public int spawnPerWave = 100;
    public int decoyProbability = 3;
    private bool markMalicious = true;
    public SerializableLevel loadLevel;
    public BaseGrid gridPrefab;

    //any values or changed must be change in LevelLookup

    public void SetLookup()
    {
        LevelLookup.spawnRate = spawnRate;
        LevelLookup.waveCount = waveCount;
        LevelLookup.spawnPerWave = spawnPerWave;
        LevelLookup.decoyProbability = decoyProbability;
    }

    public void BuildWorldGrid(WorldGrid grid)
    {
        BaseGrid newBaseGrid = Instantiate(gridPrefab);
        grid.InitGrid((int)newBaseGrid.transform.localScale.x, (int)newBaseGrid.transform.localScale.y, newBaseGrid);
    }

}
