using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefab for a Map which can be loaded
/// </summary>
/// <seealso cref="LevelLookup"/>
public class Map : MonoBehaviour {

    public bool hidden = false;
    public bool locked = true;
    [Range(1, 100)]
    public int highscoreLevelIdentifier = 0;
    public int displayOrder = 0;
    public float spawnRate = 1f;
    public int waveCount = 3;
    public int spawnPerWave = 100;
    public int decoyProbability = 3;
    public bool markMalicious = false;
    public SerializableLevel loadLevel;
    public BaseGrid gridPrefab;
    public Color arrowColor = Color.black;
    public AgentModel agentModel;

    //any values or changed must be change in LevelLookup

    public void SetLookup()
    {
        LevelLookup.spawnRate = spawnRate;
        LevelLookup.waveCount = waveCount;
        LevelLookup.spawnPerWave = spawnPerWave;
        LevelLookup.decoyProbability = decoyProbability;
        LevelLookup.markMalicious = markMalicious;
        LevelLookup.levelNumber = highscoreLevelIdentifier;
        if(agentModel != null)
        {
            LevelLookup.agentModel = agentModel.name;
        } else
        {
            LevelLookup.agentModel = LevelLookup.defaultAgentModel;
        }
    }

    public void BuildWorldGrid(WorldGrid grid)
    {
        BaseGrid newBaseGrid = Instantiate(gridPrefab);
        grid.InitGrid((int)newBaseGrid.transform.localScale.x, (int)newBaseGrid.transform.localScale.y, newBaseGrid);
    }

    public bool GetUnlocked()
    {
        if (!locked)
        {
            return true;
        }
        if (LevelUnlocking.IsUnlocked(highscoreLevelIdentifier))
        {
            return true;
        } else
        {
            return false;
        }
    }

}
