using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour {

    private ControlPrefs controlPrefs;

    private GameObject toolTips;
    private GameObject toolPrompt;
    private Text textTips;
    private Text textPrompt;

    private void Awake()
    {
        controlPrefs = GameObject.FindGameObjectWithTag("ControlPrefs").GetComponent<ControlPrefs>();
        if (controlPrefs == null)
        {
            Debug.LogError("Cannot find ControlPrefs, perhaps it was moved or the tag was not applied?");
            return;
        }

        toolTips = transform.Find("Tips").gameObject;
        toolPrompt = transform.Find("Prompt").gameObject;
        if(toolTips == null || toolPrompt == null)
        {
            Debug.LogError("Could not find the 'Tips' or 'Prompt' child!");
            return;
        }

        textTips = toolTips.transform.GetChild(0).GetComponent<Text>();
        textPrompt = toolPrompt.transform.GetChild(0).GetComponent<Text>();

        if (textTips == null || textPrompt == null)
        {
            Debug.LogError("Tips and Prompt must have Text child objects!");
            return;
        }

        SetText();

    }

    private void OnEnable()
    {
        ShowToolTips(false);
    }

    private void SetText()
    {
        textPrompt.text = "[Shift + " + controlPrefs["toggleTooltips"] + "] for tooltips";
        textTips.text = string.Format(
                        "Basic navigation:\n" +
                        "[{0}] to move selection left\n" +
                        "[{1}] to move selection right\n" +
                        "[{2}] to skip to next trait\n\n" +
                        "Advanced navigation (will skip automatically):\n" +
                        "[1-3 Number keys] to choose a trait directly\n" +
                        "[{3}] to reset selection to All\n\n" +
                        "[Shift + {4}] or click to close tooltips",
                        controlPrefs["rolodexLeftKey"],
                        controlPrefs["rolodexRightKey"],
                        controlPrefs["rolodexNextKey"],
                        controlPrefs["rolodexResetKey"],
                        controlPrefs["toggleTooltips"]);
    }

    private void ShowToolTips(bool show)
    {
        SetText();
        toolPrompt.SetActive(!show);
        toolTips.SetActive(show);
    }

    private void Update()
    {
        if (ControlPrefs.GetKeyDown("toggleTooltips"))
        {
            if (toolPrompt.activeSelf)
            {
                ShowToolTips(true);
            } else
            {
                ShowToolTips(false);
            }
        }
        if (Input.GetMouseButtonDown(0) && !toolPrompt.activeSelf)
        {
            ShowToolTips(false);
        }
    }
}
