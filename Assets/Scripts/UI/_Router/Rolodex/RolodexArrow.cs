using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RolodexArrow : MonoBehaviour {

    public bool right;
    private RolodexSelection parent;
    private Color hover;
    private Color unhover;
    private Color pressed;

    private VisualPrefs visualPrefs;
    private Graphic image;

    private bool isHoverActive;

    private void Awake()
    {
        visualPrefs = GameObject.Find("VisualPrefs").GetComponent<VisualPrefs>();
        if (visualPrefs == null)
        {
            Debug.LogError("Cannot find VisualPrefs, perhaps it was moved or renamed?");
            return;
        }
        parent = transform.parent.GetComponent<RolodexSelection>();
        if(parent == null)
        {
            Debug.LogError("Cannot find Rolodex parent of Arrow!");
        }

        image = GetComponent<Graphic>();

        hover = visualPrefs.subSelectedColor;
        unhover = visualPrefs.subDeselectedColor;
        pressed = visualPrefs.subPressedColor;

        image.color = unhover;

    }

    public void IncrementSelection()
    {
        parent.ChangeDropdown(right);
        image.color = pressed;
    }

    public void Hover(bool isHover)
    {
        image.color = (isHover) ? hover : unhover;
        isHoverActive = isHover;
    }

    public void Unpressed()
    {
        image.color = isHoverActive ? hover : unhover;
    }


}
