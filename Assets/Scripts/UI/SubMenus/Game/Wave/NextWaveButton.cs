using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NextWaveButton : GameButton
{

    public WaveController waveController;
    public Color goColor;
    public Color stopColor;

    private void Start()
    {
        SetGo(true);
    }

    public void SetGo(bool go)
    {
        Button button = transform.Find("Button").GetComponent<Button>();
        if (go)
        {
            button.GetComponent<Graphic>().color = goColor;
            button.GetComponent<Button>().interactable = true;
            button.GetComponent<EventTrigger>().enabled = true;
        } else
        {
            button.GetComponent<Graphic>().color = stopColor;
            button.GetComponent<Button>().interactable = false;
            button.GetComponent<EventTrigger>().enabled = false;
        }
    }

    public override void PerformAction()
    {
        if(!waveController.Playing){
            waveController.PlayWave(this);
            SetGo(false);
        }
    }
}
