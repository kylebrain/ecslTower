using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTips : MonoBehaviour {

    public Text text;
    public GameObject dismiss;
    public Tutorial tutorial;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(string _text = "", bool showDismiss = true)
    {
        if(!string.IsNullOrEmpty(_text))
        {
            SetText(_text);
        }
        gameObject.SetActive(true);
        tutorial.Dismiss(false);
        ShowDismiss(showDismiss);
    }

    public void ShowDismiss(bool showDismiss = true)
    {
        if (showDismiss)
        {
            dismiss.SetActive(true);
        }
        else
        {
            dismiss.SetActive(false);
        }
    }


    public void SetText(string _text)
    {
        text.text = _text;
    }
}
