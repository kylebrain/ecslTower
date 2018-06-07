using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelPanel : MonoBehaviour {
    public string LevelName
    {
        get
        {
            return levelName;
        }
        set
        {
            text.text = value;
            levelName = value;
            name = value;
        }
    }

    private string levelName;

    private Text text;

    private void Awake()
    {
        text = transform.GetChild(0).GetComponent<Text>();
    }

    public void LoadLevel()
    {
        LevelLookup.levelName = LevelName;
        SceneManager.LoadScene("Gameplay");
    }

}
