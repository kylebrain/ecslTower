using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Base class for all classes that apply an AgentAttribute
/// </summary>
public abstract class VisualAgent : NetworkBehaviour {

    /// <summary>
    /// The Attribute used by the Agent to determine traits
    /// </summary>
    /// <remarks>
    /// Used by the Router to determine whether an Agent passes a filter
    /// </remarks>
    /// <seealso cref="RouterBuilding"/>
    [SyncVar]
    public AgentAttribute Attribute;

    /// <summary>
    /// Decides how fast the Agent moves
    /// </summary>
    /// <remarks>
    /// Handled by derived classes
    /// </remarks>
    public float Speed = 0f;

    /// <summary>
    /// Sets all useable values of Agent based on the Enum values
    /// </summary>
    /// <param name="attributes">The desire attributes</param>
    public void InitializeAttributes(AgentAttribute attributes)
    {
        //model is needed for the Attributes so must create one before setting the Attributes
        CreateAgentModel();
        Attribute = attributes;
        SetColor(attributes.Color);
        SetSize(attributes.Size);
        SetSpeed(attributes.Speed);

    }

    

    /// <summary>
    /// Creates the visual AgentModel of the Agent if it needs one
    /// </summary>
    /// <seealso cref="AgentModel"/>
    protected virtual void CreateAgentModel() { }

    /// <summary>
    /// Sets the speed float based on speed Enum
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
    /// Determines a size float based on size Enum
    /// </summary>
    /// <remarks>
    /// Passes -1 to ApplySize if the size is not recognized
    /// </remarks>
    /// <param name="size">Desired size</param>
    public void SetSize(AgentAttribute.PossibleSizes size)
    {
        switch (size)
        {
            case AgentAttribute.PossibleSizes.small:
                ApplySize(0.5f);
                break;
            case AgentAttribute.PossibleSizes.medium:
                ApplySize(1);
                break;
            case AgentAttribute.PossibleSizes.large:
                ApplySize(1.5f);
                break;
            default:
                ApplySize(-1f);
                break;
        }
        
    }

    /// <summary>
    /// Handled by the derived class to set transform
    /// </summary>
    /// <param name="size">Desired size float</param>
    protected abstract void ApplySize(float size);

    /// <summary>
    /// Color set based on the Enum
    /// </summary>
    /// <param name="color">Desired color Enum</param>
    public void SetColor(AgentAttribute.PossibleColors color)
    {
        Color setColor;
        switch (color)
        {
            case AgentAttribute.PossibleColors.red:
                setColor = Color.red;
                break;
            case AgentAttribute.PossibleColors.green:
                setColor = Color.green;
                break;
            case AgentAttribute.PossibleColors.blue:
                setColor = Color.blue;
                break;
            default:
                setColor = Color.white;
                break;
        }
        ApplyColor(setColor);
    }

    /// <summary>
    /// Handles by the derived class to set the color
    /// </summary>
    /// <param name="color">Desired color</param>
    protected abstract void ApplyColor(Color color);
}
