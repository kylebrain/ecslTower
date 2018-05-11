using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(GameButton))]
public sealed class ButtonCommand: MonoBehaviour 
{
    private GameButton gameButton;
    private Button button;

    private void Awake() 
    {
        gameButton = GetComponent<GameButton>();

        if (gameButton == null) {
            Debug.LogError("ButtonCommand requires a GameButton. Please attach a script inheriting from GameButton to this game object.");
        }

        button = GetComponentInChildren<Button>();
    }

    private void Start() 
    {
        button.onClick.AddListener(gameButton.PerformAction);
    }
}
