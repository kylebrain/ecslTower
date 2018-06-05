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
    public void SetSpeed(AgentAttribute.possibleSpeeds speed)
    {
        switch (speed)
        {
            case AgentAttribute.possibleSpeeds.slow:
                Speed = 1.5f;
                break;
            case AgentAttribute.possibleSpeeds.normal:
                Speed = 3.5f;
                break;
            case AgentAttribute.possibleSpeeds.fast:
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
    public void SetSize(AgentAttribute.possibleSizes size)
    {
        Vector3 newScale = Vector3.one;
        switch (size)
        {
            case AgentAttribute.possibleSizes.small:
                newScale = new Vector3(0.25f, 1, 0.5f);
                break;
            case AgentAttribute.possibleSizes.medium:
                newScale = new Vector3(0.5f, 1, 1f);
                break;
            case AgentAttribute.possibleSizes.large:
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
    /// <param name="color">Desired Color</param>
    public virtual void SetColor(AgentAttribute.possibleColors color)
    {
        Renderer rend = GetComponent<Renderer>();
        Material selectedMaterial = null;
        switch (color)
        {
            case AgentAttribute.possibleColors.red:
                selectedMaterial = Resources.Load<Material>("Agent/Red");
                break;
            case AgentAttribute.possibleColors.green:
                selectedMaterial = Resources.Load<Material>("Agent/Green");
                break;
            case AgentAttribute.possibleColors.blue:
                selectedMaterial = Resources.Load<Material>("Agent/Blue");
                break;
            default:
                Debug.LogError("Agent color not recognized!");
                break;
        }
        if (selectedMaterial != null)
        {
            rend.material = selectedMaterial;
        }
        else
        {
            Debug.LogError("Could not find Agent material. Perhaps it was moved?");
        }
    }
}
