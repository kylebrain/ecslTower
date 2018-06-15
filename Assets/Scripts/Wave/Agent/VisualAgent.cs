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
        CreateAgentModel();
        Attribute = attributes;
        SetColor(attributes.Color);
        SetSize(attributes.Size);
        SetSpeed(attributes.Speed);
    }

    protected virtual void CreateAgentModel() { }

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
        switch (size)
        {
            case AgentAttribute.PossibleSizes.small:
                ApplySize(0.5f);
                //newScale = new Vector3(0.25f, 1, 0.5f);
                break;
            case AgentAttribute.PossibleSizes.medium:
                ApplySize(1);
                //newScale = new Vector3(0.5f, 1, 1f);
                break;
            case AgentAttribute.PossibleSizes.large:
                ApplySize(1.5f);
                //newScale = new Vector3(0.75f, 1, 1.5f);
                break;
            default:
                ApplySize(-1f);
                //newScale = Vector3.one;
                break;
        }
        
    }

    protected abstract void ApplySize(float size);

    //change also in RingDisplayAgent and Agent

    /// <summary>
    /// Sets Render Material based on color Enum
    /// </summary>
    /// <remarks>
    /// Should be overwritten by RingDisplayAgent with modifies the Graphic instead of the material
    /// </remarks>
    /// <seealso cref="RingDisplayAgent"/>
    /// <param name="color">Desired Color</param>
    public abstract void SetColor(AgentAttribute.PossibleColors color);
}
