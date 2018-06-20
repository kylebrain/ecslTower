using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    //money
    public int startingAmount = 0;
    public static int Money = 0;
    public Text moneyText;


    public static readonly int MaxHealth = 100;

    //health
    public static int Health
    {
        get
        {
            return localHealth;
        }
        set
        {
            localHealth = value;
            if (localHealth > MaxHealth)
            {
                localHealth = MaxHealth;
            }

            if (localHealth < 0)
            {
                localHealth = 0;
            }
        }
    }

    private static int localHealth;

    public Text healthText;
    public Slider healthSlider;

    public EndScreen endScreen;

    private static Score instance;

    private void Awake()
    {
        instance = this;
        Money = startingAmount;
        Health = MaxHealth;
    }

    private void Update()
    {
        moneyText.text = "$" + Money + ".00";

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        if (Health < 0)
        {
            Health = 0;
        }

        healthText.text = Health.ToString();
        healthSlider.value = Health;
    }

    public static void GameLost()
    {
        instance.endScreen.EndGame(false);
    }
}
