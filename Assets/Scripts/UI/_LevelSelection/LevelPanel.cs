using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelPanel : MonoBehaviour
{
    public string LevelName
    {
        get
        {
            return levelName;
        }
        set
        {
            levelNameText.text = value;
            levelName = value;
            name = value;
        }
    }

    private string levelName;
    private int levelNumber;
    public string publicCode;

    private Text levelNameText;
    private Text highscoreText;

    private void Awake()
    {
        levelNameText = transform.GetChild(0).GetComponent<Text>();
        highscoreText = transform.GetChild(1).GetComponent<Text>();
    }

    public void LoadLevel()
    {
        LevelLookup.levelNumber = levelNumber;
        LevelLookup.levelName = LevelName;
        SceneManager.LoadScene("Gameplay");
    }

    public void Init(int levelNum, string levelName, bool unlocked, string _publicCode)
    {
        levelNumber = levelNum;
        LevelName = levelName;
        publicCode = _publicCode;
        if (!unlocked)
        {
            GetComponent<Button>().interactable = false;
            transform.Find("Chain").gameObject.SetActive(true);
        } else
        {
            GetComponent<Button>().interactable = true;
        }
    }

    public void AddHighScore(Highscore highscore)
    {
        if (highscoreText != null)
        {
            highscoreText.text = "Highscore:\n" + highscore.username + " - " + highscore.score;
        }
    }

}
