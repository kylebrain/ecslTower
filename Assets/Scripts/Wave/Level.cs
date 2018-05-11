using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Level
{
    public string levelName;
    public List<SerializableWavePath> wavePaths = new List<SerializableWavePath>();
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

    public void ClearLevel()
    {
        wavePaths = new List<SerializableWavePath>();
    }

    public void DeleteLevel()
    {
        ClearLevel();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + PathName, FileMode.Open);
        bf.Serialize(file, new Level());
        file.Close();
    }

    public void SetLevel(List<WavePath> wavePathList)
    {
        ClearLevel();
        PathName = levelName;
        foreach (WavePath path in wavePathList)
        {
            wavePaths.Add(new SerializableWavePath(path));
        }
    }

    public void SetLevel(List<SerializableWavePath> wavePathList)
    {
        ClearLevel();
        PathName = levelName;
        foreach (SerializableWavePath path in wavePathList)
        {
            wavePaths.Add(path);
        }
    }

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
            Debug.Log(string.Format("File doesn't exist at path: {0}{1}", Application.persistentDataPath, "/save_game.dat"));
            return null;
        }
    }

}
