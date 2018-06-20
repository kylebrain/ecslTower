using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Model of the Agent
/// </summary>
/// <remarks>
/// Must have an encapsulating Collider, child objects should NOT have a Collider
/// </remarks>
[RequireComponent(typeof(Collider))]
public abstract class AgentModel : MonoBehaviour {
    /// <summary>
    /// Derived class sets color according to its set up
    /// </summary>
    /// <param name="color">Desired color</param>
    public abstract void SetModelColor(Color color);
    /// <summary>
    /// Derived class sets size according to its set up
    /// </summary>
    /// <param name="color">Desired size</param>
    public abstract void SetModelSize(float size);
}
