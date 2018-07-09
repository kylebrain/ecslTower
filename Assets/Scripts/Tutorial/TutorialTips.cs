using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTips : MonoBehaviour {

    public Text text;
    public GameObject dismiss;
    public Text dismissText;
    public Tutorial tutorial;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(ControlPrefs.GetKeyDown("dismissTutorial") && dismiss.activeSelf)
        {
            tutorial.Dismiss(true);
            Hide();
        }
    }

    public void Show(string _text = "", bool showDismiss = true)
    {
        if(!string.IsNullOrEmpty(_text))
        {
            SetText(_text);
        }
        AudioManager.Play("TutorialPrompt");
        gameObject.SetActive(true);
        tutorial.Dismiss(false);
        ShowDismiss(showDismiss);
    }

    public void ShowDismiss(bool showDismiss = true)
    {
        dismissText.text = "Dismiss [" + ControlPrefs.GetKey("dismissTutorial") + "]";
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
