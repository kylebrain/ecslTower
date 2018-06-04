using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RingDisplayAgent : VisualAgent
{

    public Vector2 center;
    public float startingRotation;
    public float radius;
    public float rotationMod = 0.5f;

    private const float sizeMod = 30f;

    public float viewRotation;

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
    }

    private void Update()
    {
        if (Speed <= 0)
        {
            Rotation = 0f;
        }
        transform.localPosition = new Vector3(center.x + radius * Mathf.Cos(Rotation * Mathf.Deg2Rad), center.y + radius * Mathf.Sin(Rotation * Mathf.Deg2Rad));
        transform.localEulerAngles = new Vector3(0f, 0f, Rotation);
        Rotation += Speed * rotationMod * Time.deltaTime;
        viewRotation = Rotation;
    }

    protected override void ApplySize(Vector3 size)
    {
        GetComponent<RectTransform>().sizeDelta = size * sizeMod;
    }

    public override void SetColor(AgentAttribute.possibleColors color)
    {
        Graphic image = GetComponent<Graphic>();
        switch (color)
        {
            case AgentAttribute.possibleColors.red:
                image.color = Color.red;
                break;
            case AgentAttribute.possibleColors.green:
                image.color = Color.green;
                break;
            case AgentAttribute.possibleColors.blue:
                image.color = Color.blue;
                break;
            default:
                image.color = Color.white;
                break;
        }
    }
}
