using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualAgent : MonoBehaviour {

    //have this retrieve an Agent from VisualPrefs on Start or in InitializeAtrr

    public AgentAttribute Attribute;
    public float Speed = 0f;

    /// <summary>
    /// Sets all useable values of Agent based on the Enum values
    /// </summary>
    /// <param name="attributes">The desire attributes</param>
    public void InitializeAttributes(AgentAttribute attributes)
    {
        Attribute = attributes;
        SetColor(attributes.Color);
        SetSize(attributes.Size);
        SetSpeed(attributes.Speed);
    }

    /// <summary>
    /// Sets the NavAgent speed based on speed Enum
    /// </summary>
    /// <param name="speed">Desired speed</param>
    public void SetSpeed(AgentAttribute.PossibleSpeeds speed)
    {
        switch (speed)
        {
            case AgentAttribute.PossibleSpeeds.slow:
                Speed = 1.5f;
                break;
            case AgentAttribute.PossibleSpeeds.normal:
                Speed = 3.5f;
                break;
            case AgentAttribute.PossibleSpeeds.fast:
                Speed = 5.5f;
                break;
            default:
                Speed = 0f;
                break;
        }
    }

    /// <summary>
    /// Sets the Transform scale based on size Enum
    /// </summary>
    /// <param name="size">Desired size</param>
    public void SetSize(AgentAttribute.PossibleSizes size)
    {
        Vector3 newScale = Vector3.one;
        switch (size)
        {
            case AgentAttribute.PossibleSizes.small:
                newScale = new Vector3(0.25f, 1, 0.5f);
                break;
            case AgentAttribute.PossibleSizes.medium:
                newScale = new Vector3(0.5f, 1, 1f);
                break;
            case AgentAttribute.PossibleSizes.large:
                newScale = new Vector3(0.75f, 1, 1.5f);
                break;
            default:
                newScale = Vector3.one;
                break;
        }
        ApplySize(newScale);
    }

    protected abstract void ApplySize(Vector3 size);

    //change also in RingDisplayAgent

    /// <summary>
    /// Sets Render Material based on color Enum
    /// </summary>
    /// <remarks>
    /// Should be overwritten by RingDisplayAgent with modifies the Graphic instead of the material
    /// </remarks>
    /// <seealso cref="RingDisplayAgent"/>
    /// <param name="color">Desired Color</param>
    public virtual void SetColor(AgentAttribute.PossibleColors color)
    {
        Renderer rend = GetComponent<Renderer>();
        switch (color)
        {
            case AgentAttribute.PossibleColors.red:
                rend.material.SetColor("_Color", Color.red);
                break;
            case AgentAttribute.PossibleColors.green:
                rend.material.SetColor("_Color", Color.green);
                break;
            case AgentAttribute.PossibleColors.blue:
                rend.material.SetColor("_Color", Color.blue);
                break;
            default:
                rend.material.SetColor("_Color", Color.white);
                break;
        }
    }
}
