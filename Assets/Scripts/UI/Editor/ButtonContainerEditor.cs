using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonConatiner))]
public class ButtonContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector gui so the user has acces to the properties
        DrawDefaultInspector();

        // Get the button container component
        ButtonConatiner buttonConatiner = (ButtonConatiner)target;

        if (GUILayout.Button("Resize and position buttons"))
        {
            buttonConatiner.ResizeContainerAndPositionButtons();
        }
    }
}
