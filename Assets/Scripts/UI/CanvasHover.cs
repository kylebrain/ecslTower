using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool Over;
    private bool selfOver;

    /// <summary>
    /// Basic workaround so that the menu does not close when clicked on, sets Over to true
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Over = true;
        selfOver = true;
    }

    /// <summary>
    /// Basic workaround so that the menu does not close when clicked on, sets Over to false
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Over = false;
        selfOver = false;
    }

    private void OnDisable()
    {
        if (selfOver)
        {
            Over = false;
            selfOver = false;
        }
    }
}
