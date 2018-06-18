using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDisplay : MonoBehaviour
{
    public Text[] textList;
    private int tempScoreDiff = 50;
    private string[] bogeyNames = { "Michael", "Lisa", "John", "Mary", "David", "Karen", "James", "Kimberly", "Robert", "Susan" };

    void Start()
    {
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].text = i + 1 + ". Fetching...";
        }
    }
    public void OnHighscoresDownloaded(Highscore[] highscoreList)
    {
        for (int i = 0; i < textList.Length; i++)
        {
            textList[i].text = i + 1 + ". ";
            if (i < highscoreList.Length)
            {
                textList[i].text += highscoreList[i].username + " - " + highscoreList[i].score;
            } else
            {
                textList[i].text += bogeyNames[i] + " - " + (highscoreList[highscoreList.Length - 1].score - tempScoreDiff * (i - highscoreList.Length + 1));
            }
        }
    }

}
