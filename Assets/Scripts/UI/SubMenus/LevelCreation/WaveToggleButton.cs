using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaveToggleButton : GameButton
{
    private WaveManager waveManager;

    private Color enableColor;
    private Color disableColor;

    private VisualPrefs visualPrefs;

    private void Start()
    {
        visualPrefs = GameObject.Find("VisualPrefs").GetComponent<VisualPrefs>();
        if (visualPrefs == null)
        {
            Debug.LogError("Cannot find VisualPrefs, perhaps it was moved or renamed?");
            return;
        }
        waveManager = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>();
        if(waveManager == null)
        {
            Debug.LogError("Cannot find WaveManager, perhaps it was moved or retagged?");
            return;
        }
        enableColor = visualPrefs.enableColor;
        disableColor = visualPrefs.disableColor;

        //if I need to make more toggle buttons I should make it a parent class of DisableButton

        UpdateButtonColor();
    }
    private void UpdateButtonColor()
    {
        if (waveManager.enablePathEditing)
        {
            transform.Find("Button").GetComponent<Graphic>().color = enableColor;
        }
        else
        {
            transform.Find("Button").GetComponent<Graphic>().color = disableColor;
        }
    }
    public override void PerformAction()
    {
        waveManager.enablePathEditing = !waveManager.enablePathEditing;
        UpdateButtonColor();
    }
}
