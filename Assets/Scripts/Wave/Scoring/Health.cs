using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public EndScreen endScreen;

    public static readonly int maxHealth = 100;

    public static int health
    {
        get
        {
            return localHealth;
        }
        set
        {
            localHealth = value;
            if (localHealth > maxHealth)
            {
                localHealth = maxHealth;
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

    private void Awake()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if(health < 0)
        {
            health = 0;
        }

        healthText.text = health.ToString();
        healthSlider.value = health;
    }

    public void GameLost()
    {
        endScreen.EndGame(false);
    }

}
