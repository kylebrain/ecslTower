using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonGlow : MonoBehaviour
{
    private Image image;
    private Color normalColor;
    private Color hoverColor;
    private Color clickDownColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        Color color = image.color;
        normalColor = new Color(color.r, color.g, color.b, 0.5f);
        hoverColor = new Color(color.r, color.g, color.b, 0.9f);
        clickDownColor = new Color(color.r, color.g, color.b, 0.2f);

        Debug.Log("Normal Color: " + normalColor);
        Debug.Log("Hover  Color: " + hoverColor);
        Debug.Log("Click  Color: " + clickDownColor);
    }

    public void SetNormalColor()
    {
        image.color = normalColor;
    }

    public void SetHoverColor()
    {
        image.color = hoverColor;
    }

    public void SetClickDownColor()
    {
        image.color = clickDownColor;
    }
}
