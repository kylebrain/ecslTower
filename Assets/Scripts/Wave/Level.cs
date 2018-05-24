using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Level
{
    /// <summary>
    /// Set in inspector, can run into overriding issues if two levels are named the same
    /// </summary>
    public string levelName;
    /// <summary>
    /// List of SerializableWavePaths that represent each WavePath in a level
    /// </summary>
    public List<SerializableWavePath> wavePaths = new List<SerializableWavePath>();
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
    }

    /// <summary>
    /// Clears the SWavePaths and the file in which they were stored
    /// </summary>
    public void DeleteLevel()
    {
        ClearLevel();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + PathName, FileMode.Open);
        bf.Serialize(file, new Level());
        file.Close();
    }

    /// <summary>
    /// Sets the SWavePath List based on a passed WavePath List
    /// </summary>
    /// <param name="wavePathList">To be converted</param>
    public void SetLevel(List<WavePath> wavePathList)
    {
        ClearLevel();
        PathName = levelName;
        foreach (WavePath path in wavePathList)
        {
            wavePaths.Add(new SerializableWavePath(path));
        }
    }

    /// <summary>
    /// Sets the SWavePath List based on another SWavePath List
    /// </summary>
    /// <param name="wavePathList">To be copied</param>
    public void SetLevel(List<SerializableWavePath> wavePathList)
    {
        ClearLevel();
        PathName = levelName;
        foreach (SerializableWavePath path in wavePathList)
        {
            wavePaths.Add(path);
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
    public List<SerializableWavePath> LoadLevel()
    {
        PathName = levelName;
        if (File.Exists(Application.persistentDataPath + PathName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + PathName, FileMode.Open);
            Level loadLevel = (Level)bf.Deserialize(file);
            file.Close();
            wavePaths = loadLevel.wavePaths;
            return wavePaths;
        }
        else
        {
            Debug.LogError(string.Format("File doesn't exist at path: {0}{1}", Application.persistentDataPath, "/save_game.dat"));
            Debug.LogError("Enable path editing and press 'R' to save a path if it is already loaded internally.");
            return null;
        }
    }

}
