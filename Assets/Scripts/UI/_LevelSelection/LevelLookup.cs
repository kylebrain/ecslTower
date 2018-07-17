using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static values which are set through the map, but in MapDisplay
/// </summary>
/// <seealso cref="Map"/>
public static class LevelLookup {

    public static int levelNumber = 0;
    public static readonly string defaultLevelName = "DEFAULT_VALUE";
    public static string levelName = defaultLevelName;
    public static float spawnRate = 1f;
    public static int waveCount = 3;
    public static int spawnPerWave = 100;
    public static int decoyProbability = 3;
    public static int _2starScore = 2000;
    public static int _3starScore = 3000;
    public static bool markMalicious = false;
    public static string arrowColor = "#000";
    public static readonly string defaultAgentModel = "DefaultAgentModel";
    public static string agentModel = defaultAgentModel;

    //statics added here must be reflected in Map

}
