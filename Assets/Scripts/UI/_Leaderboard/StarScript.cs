using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarScript : MonoBehaviour
{

    public Image[] stars = new Image[3];
    public Text starText;
    public Color disabledColor;
    public Color enabledColor;

    public void Display(int score)
    {
        int numberDisplayed;
        int nextScore = -1;
        if (score >= LevelLookup._3starScore)
        {
            numberDisplayed = 3;
        }
        else if (score >= LevelLookup._2starScore)
        {
            nextScore = LevelLookup._3starScore;
            numberDisplayed = 2;
        }
        else if (score >= Score.StartingAmount)
        {
            nextScore = LevelLookup._2starScore;
            numberDisplayed = 1;
        }
        else
        {
            nextScore = Score.StartingAmount;
            numberDisplayed = 0;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (i < numberDisplayed)
            {
                stars[i].color = enabledColor;
            }
            else
            {
                stars[i].color = disabledColor;
            }
        }

        if (numberDisplayed < 3)
        {
            starText.text = "Only " + (nextScore - score) + " more for " + (numberDisplayed + 1) + " thumb" + ((numberDisplayed + 1) == 1 ? "" : "s") + " up!";
        }
        else
        {
            starText.text = "Congratulations!";
        }

        gameObject.SetActive(true);

    }

}
