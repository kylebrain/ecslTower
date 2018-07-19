using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectedLevel : NetworkBehaviour {

    [SyncVar(hook = "UpdateSelectedLevel")]
    public string SelectedLevelName = LevelLookup.defaultLevelName;

    public Text levelText;

    public static SelectedLevel instance;

    private void Start()
    {
        instance = this;
        UpdateSelectedLevel(SelectedLevelName);
    }


    public void UpdateSelectedLevel(string _selectedLevelName)
    {
        if (_selectedLevelName != LevelLookup.defaultLevelName)
        {
            levelText.text = _selectedLevelName;
            LevelLookup.levelName = _selectedLevelName;
            SelectedLevelName = _selectedLevelName;
        } else
        {
            levelText.text = "None";
        }
    }
}
