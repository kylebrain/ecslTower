using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfo : MonoBehaviour {

    private Text waveNumber;
    private Text countDown;
    private Text incomingAgents;

    private string countDownText;

    private void Awake()
    {
        waveNumber = transform.Find("WaveNumber").GetChild(0).GetComponent<Text>();
        countDown = transform.Find("CountDown").GetChild(0).GetComponent<Text>();
        incomingAgents = transform.Find("IncomingAgents").GetChild(0).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (gameObject.activeSelf)
        {
            UpdateInfo();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetCountDownText(true);
        UpdateInfo();
    }

    //if wave is false countdown to next wave
    public void SetCountDownText(bool wave)
    {
        countDownText = wave ? "Time remaining:\n" : "Next wave in:\n";
    }

    private void UpdateInfo()
    {
        waveNumber.text = "Wave: " + WaveController.WaveCount + "/" + LevelLookup.waveCount;
        countDown.text = countDownText + WaveController.SecondsLeft.ToString("N1") + "s";
        incomingAgents.text = "Incoming Agents:\n" + WaveController.AgentsRemaining;
    }

}
