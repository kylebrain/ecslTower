using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public static int score = 0;
    public Text scoreText;

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }
}
