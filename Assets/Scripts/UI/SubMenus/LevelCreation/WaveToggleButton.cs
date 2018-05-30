using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaveToggleButton : GameButton
{
    public WaveManager waveManager;
    private void Start()
    {
        UpdateButtonColor();
    }
    private void UpdateButtonColor()
    {
        if (waveManager.enablePathEditing)
        {
            transform.Find("Button").GetComponent<Graphic>().color = Color.green;
        }
        else
        {
            transform.Find("Button").GetComponent<Graphic>().color = Color.red;
        }
    }
    public override void PerformAction()
    {
        waveManager.enablePathEditing = !waveManager.enablePathEditing;
        UpdateButtonColor();
    }
}
