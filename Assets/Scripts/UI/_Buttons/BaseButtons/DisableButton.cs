using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DisableButton : GameButton {

    public bool ButtonEnabled
    {
        get
        {
            return buttonEnabled;
        }
    }

    private bool buttonEnabled;

    private Color enableColor;
    private Color disableColor;

    private VisualPrefs visualPrefs;

    private void Start()
    {
        visualPrefs = GameObject.Find("VisualPrefs").GetComponent<VisualPrefs>();
        if(visualPrefs == null)
        {
            Debug.LogError("Cannot find VisualPrefs, perhaps it was moved or renamed?");
            return;
        }
        enableColor = visualPrefs.enableColor;
        disableColor = visualPrefs.disableColor;

        //defaults to enabled
        SetEnable(true);

        DerivedStart();
    }

    public void SetEnable(bool enable)
    {
        Button button = GetButton;
        if (enable)
        {
            buttonEnabled = true;
            button.GetComponent<Graphic>().color = enableColor;
            button.interactable = true;
            button.GetComponent<EventTrigger>().enabled = true;
        }
        else
        {
            buttonEnabled = false;
            //set the transition duration to 0 before disabling
            button.interactable = false;
            button.GetComponent<Graphic>().color = disableColor;
            button.GetComponent<EventTrigger>().enabled = false;
            GetButtonGlow.SetHoverColor(false);
        }
    }

    public abstract override void PerformAction();

    protected virtual void DerivedStart() { }

}
