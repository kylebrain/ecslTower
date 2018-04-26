using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelButton : GameButton
{
    public RectTransform LoadLevelUI;
    
    public override void PerformAction()
    {
        LoadLevelUI.gameObject.SetActive(!LoadLevelUI.gameObject.activeSelf);
    }
}
