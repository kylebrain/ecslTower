using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SerializableLevel
{
    /// <summary>
    /// Set in inspector, can run into overriding issues if two levels are named the same
    /// </summary>
    public string levelName;
    /// <summary>
    /// List of SerializableWavePaths that represent each WavePath in a level
    /// </summary>
    public List<SerializableWavePath> wavePaths = new List<SerializableWavePath>();

    public List<SerializableEndArea> endAreas = new List<SerializableEndArea>();

    /// <summary>
    /// Absolute name of the file based on the levelName
    /// </summary>
    private string PathName
    {
        get
        {
            return pathName;
        }
        set
        {
            pathName = "/level_" + value + ".dat";
        }
    }
    private string pathName = "";

    /// <summary>
    /// Resets the Level, but does not clear file
    /// </summary>
    public void ClearLevel()
    {
        wavePaths = new List<SerializableWavePath>();
        endAreas = new List<SerializableEndArea>();
    }

    /// <summary>
    /// Clears the SWavePaths and the file in which they were stored
    /// </summary>
    public void DeleteLevel()
    {
        ClearLevel();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + PathName, FileMode.Open);
        bf.Serialize(file, new SerializableLevel());
        file.Close();
    }

    /// <summary>
    /// Sets the SWavePath List based on a passed WavePath List
    /// </summary>
    /// <param name="wavePathList">To be converted</param>
    public void SetLevel(List<WavePath> wavePathList, List<GridArea> Sources, List<GridArea> Sinks)
    {
        ClearLevel();
        PathName = levelName;
        foreach (WavePath path in wavePathList)
        {
            wavePaths.Add(new SerializableWavePath(path));
        }
        foreach(GridArea gridArea in Sources)
        {
            endAreas.Add(new SerializableEndArea(gridArea, false));
        }
        foreach (GridArea gridArea in Sinks)
        {
            endAreas.Add(new SerializableEndArea(gridArea, true));
        }
    }

    /// <summary>
    /// Sets the SWavePath List based on another SWavePath List
    /// </summary>
    /// <param name="wavePathList">To be copied</param>
    public void SetLevel(SerializableLevel level)
    {
        ClearLevel();
        PathName = levelName;
        foreach (SerializableWavePath path in level.wavePaths)
        {
            wavePaths.Add(path);
        }
        foreach (SerializableEndArea endArea in level.endAreas)
        {
            endAreas.Add(endArea);
        }
    }

    /// <summary>
    /// Saves the Level to a file based on the levelName
    /// </summary>
    public void SaveLevel()
    {
        if (wavePaths.Count > 0 && pathName != "")
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + PathName);
            bf.Serialize(file, this);
            file.Close();
        }
    }

    /// <summary>
    /// Loads the Level from the file based on the levelName
    /// </summary>
    /// <returns></returns>
    public SerializableLevel LoadLevel()
    {
        PathName = levelName;
        if (File.Exists(Application.persistentDataPath + PathName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + PathName, FileMode.Open);
            SerializableLevel loadLevel = (SerializableLevel)bf.Deserialize(file);
            file.Close();
            wavePaths = loadLevel.wavePaths;
            endAreas = loadLevel.endAreas;
            return loadLevel;
        }
        else
        {
            Debug.LogError(string.Format("File doesn't exist at path: {0}{1}", Application.persistentDataPath, PathName));
            Debug.LogError("Enable path editing and press 'R' to save a path if it is already loaded internally.");
            return null;
        }
    }

}
