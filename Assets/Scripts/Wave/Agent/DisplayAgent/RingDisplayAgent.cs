using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a visual representation of the VisualAgent on a Canvas
/// </summary>
[RequireComponent(typeof(Graphic))]
public class RingDisplayAgent : VisualAgent
{
    /// <summary>
    /// When rotation is set to 0 the VisualAgent is placed at this rotation
    /// </summary>
    public float startingRotation;
    /// <summary>
    /// The radius of rotation
    /// </summary>
    public float radius;
    /// <summary>
    /// How fast the VisualAgent rotates arround the radius
    /// </summary>
    public float rotationMod = 0.5f;
    /// <summary>
    /// How much the VisualAgent is scaled proportional to the passed size float
    /// </summary>
    /// <seealso cref="VisualAgent"/>
    public float sizeMod = 30f;

    /// <summary>
    /// The center of the ring
    /// </summary>
    /// <remarks>
    /// Set to its initial positon at start
    /// Must move center to move the ring
    /// </remarks>
    public Vector2 center;

    public RectTransform rectTransform
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }

    public float GetRotation
    {
        get
        {
            return rotation;
        }
    }

    /// <summary>
    /// Degrees ranges [0,360)
    /// </summary>
    private float Rotation
    {
        get
        {
            return rotation;
        }
        set
        {
            float current = value;
            while (current >= 360f)
            {
                current -= 360f;
            }
            while(current < 0f)
            {
                current += 360f;
            }
            rotation = current;
        }
    }

    /// <summary>
    /// Stores Rotation
    /// </summary>
    private float rotation;

    private void Start()
    {
        Rotation = startingRotation;
        center = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        //if the agent is not moving, its position is reset to the startingRotation
        if (Speed <= 0)
        {
            Rotation = startingRotation;
        }
        //rectTransform.anchoredPosition = center;

        //places the VisualAgent based on its polar coordinates
        UpdatePosition();
        //rotates the Agent to "follow" the path its moving in
        transform.localEulerAngles = new Vector3(0f, 0f, Rotation);
        //increases the rotation
        Rotation += Speed * rotationMod * Time.deltaTime;
        UpdateAction();
    }

    protected void UpdatePosition()
    {
        rectTransform.anchoredPosition = new Vector3(center.x + radius * Mathf.Cos(Rotation * Mathf.Deg2Rad), center.y + radius * Mathf.Sin(Rotation * Mathf.Deg2Rad));
    }

    protected virtual void UpdateAction() { }

    /// <summary>
    /// Changes the Graphic sizeDelta
    /// </summary>
    /// <param name="size">Desire size</param>
    protected override void ApplySize(float size)
    {
        Vector2 proportions;
        //if size is unknown (dontCare) the agent is a square
        if(size < 0)
        {
            proportions = Vector2.one;
        } else
        {
            //scales as a rectangle if the size is recognized
            proportions = new Vector2(0.5f, 1) * size;
        }
        GetComponent<RectTransform>().sizeDelta = proportions * sizeMod;
    }

    /// <summary>
    /// Sets the Graphic color
    /// </summary>
    /// <param name="color">Desire color</param>
    protected override void ApplyColor(Color color)
    {
        Graphic image = GetComponent<Graphic>();
        image.color = color;
    }
}
