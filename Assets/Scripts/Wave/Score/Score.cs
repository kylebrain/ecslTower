using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public int startingAmount = 0;
    public static int score = 0;
    public Text scoreText;

    private void Awake()
    {
        score = startingAmount;
    }

    private void Update()
    {
        scoreText.text = "$" + score + ".00";
    }
}
