using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RolodexArrow : MonoBehaviour
{

    public bool right;
    private RolodexSelection parent;
    private Color hover;
    private Color unhover;
    private Color pressed;
    private Color disabled;

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
        if (parent == null)
        {
            Debug.LogError("Cannot find Rolodex parent of Arrow!");
        }

        image = GetComponent<Graphic>();

        hover = visualPrefs.subSelectedColor;
        unhover = visualPrefs.subDeselectedColor;
        pressed = visualPrefs.subPressedColor;
        disabled = visualPrefs.subDisabledColor;

        image.color = unhover;

    }

    public void IncrementSelection()
    {
        if (!parent.Disabled)
        {
            parent.ChangeDropdown(right);
            image.color = pressed;
        }
    }

    public void Hover(bool isHover)
    {
        if (!parent.Disabled)
        {
            image.color = (isHover) ? hover : unhover;
        }
        isHoverActive = isHover;
    }

    public void Unpressed()
    {
        if (!parent.Disabled)
        {
            image.color = isHoverActive ? hover : unhover;
        }
    }

    public void Disabled(bool isDisabled)
    {
        if (isDisabled)
        {
            image.color = disabled;
        }
        else
        {
            if (isHoverActive)
            {
                image.color = hover;
            }
            else
            {
                image.color = unhover;
            }
        }
    }

}
