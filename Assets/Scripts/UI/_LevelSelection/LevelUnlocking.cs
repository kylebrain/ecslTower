using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnlocking : MonoBehaviour {

    private static List<int> unlocked = new List<int>();

    private void Awake()
    {
        string unlockedString = PlayerPrefs.GetString("unlockedLevels");
        if(unlockedString != "")
        {
            for(int i = 0; i < unlockedString.Length; i++)
            {
                int levelNumber = System.Int32.Parse(unlockedString[i].ToString());
                AddToUnlocked(levelNumber);
            }
        }
    }

    public static void AddToUnlocked(int levelNumber)
    {
        if (!IsUnlocked(levelNumber))
        {
            unlocked.Add(levelNumber);
        }
        string unlockedString = "";
        foreach(int i in unlocked)
        {
            unlockedString += i.ToString();
        }
        PlayerPrefs.SetString("unlockedLevels", unlockedString);
    }

    public static List<int> GetUnlocked()
    {
        return unlocked;
    }

    public static bool IsUnlocked(int levelNumber)
    {
        return unlocked.Contains(levelNumber);
    }

    public static void ResetUnlocked()
    {
        PlayerPrefs.DeleteKey("unlockedLevels");
        unlocked.Clear();
    }
}
