using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HackCooldown : MonoBehaviour {

    public Text deployCountText;

    private int deployCount = 0;

    Slider slider;
    // in seconds
    const float cycleDuration = 20; // change to 10
    const int purchasePercentage = 20;
    float currentTime = 0;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if(slider != null)
        {
            slider.maxValue = cycleDuration;
        }
    }

    private void Update()
    {
        if (slider != null)
        {
            slider.value = currentTime;
        }

        if (currentTime >= cycleDuration)
        {

            CompleteCycle();

            currentTime %= cycleDuration;
        }

        currentTime += Time.deltaTime;

        

        
    }

    public void CompleteCycle()
    {
        DeployCount(1);
    }

    public int DeployCount(int toAdd = 0)
    {
        deployCount += toAdd;
        deployCountText.text = deployCount.ToString();
        return deployCount;
    }

    public void AdvanceTime()
    {
        if(Attacker.HackerCurrency >= Attacker.AdvanceTimeCost)
        {
            currentTime += (purchasePercentage / 100f) * cycleDuration;
            Attacker.HackerCurrency -= Attacker.AdvanceTimeCost;
        }
        else
        {
            Debug.LogWarning("Not enough money to speed up hack progress!");
        }

    }

}
