using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDisplay : MonoBehaviour
{
    public Text[] textList;

    void Start()
    {
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].text = i + 1 + ". Fetching...";
        }
    }
    public void OnHighscoresDownloaded(List<Highscore> highscoreList)
    {
        if (highscoreList == null)
        {
            highscoreList = new List<Highscore>();
        }
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].text = i + 1 + ". ";
            if (i < highscoreList.Count)
            {
                textList[i].text += highscoreList[i].username + " - " + highscoreList[i].score;
            }
        }
    }

}
