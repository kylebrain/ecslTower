using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RingDisplayAgent : VisualAgent
{

    private Vector2 center;
    public float startingRotation;
    public float radius;
    public float rotationMod = 0.5f;

    public float sizeMod = 30f;

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

    private float rotation;

    private void Start()
    {
        Rotation = startingRotation;
        center = transform.localPosition;
    }

    private void Update()
    {
        if (Speed <= 0)
        {
            Rotation = startingRotation;
        }
        transform.localPosition = new Vector3(center.x + radius * Mathf.Cos(Rotation * Mathf.Deg2Rad), center.y + radius * Mathf.Sin(Rotation * Mathf.Deg2Rad));
        transform.localEulerAngles = new Vector3(0f, 0f, Rotation);
        Rotation += Speed * rotationMod * Time.deltaTime;
    }

    protected override void ApplySize(Vector3 size)
    {
        GetComponent<RectTransform>().sizeDelta = size * sizeMod;
    }

    public override void SetColor(AgentAttribute.PossibleColors color)
    {
        Graphic image = GetComponent<Graphic>();
        switch (color)
        {
            case AgentAttribute.PossibleColors.red:
                image.color = Color.red;
                break;
            case AgentAttribute.PossibleColors.green:
                image.color = Color.green;
                break;
            case AgentAttribute.PossibleColors.blue:
                image.color = Color.blue;
                break;
            default:
                image.color = Color.white;
                break;
        }
    }
}
