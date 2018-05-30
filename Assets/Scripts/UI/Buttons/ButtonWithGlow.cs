using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonWithGlow : MonoBehaviour
{
    public ButtonGlow buttonGlow;

    private Button button;

    private void Awake()
    {
        //button = GetComponent<Button>();
        //add back to increase functionality
    }

    private void Start()
    {
        if (buttonGlow == null)
        {
            Debug.Log("There is no assigned button glow component");
        }
        // TODO: Set button events
    }
}
