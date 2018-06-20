using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default template Model for new maps
/// </summary>
public class DefaultAgentModel : AgentModel {

    /// <summary>
    /// Sets the color
    /// </summary>
    /// <param name="color">Desired color</param>
    public override void SetModelColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }

    /// <summary>
    /// Prefab is scaled rectangular based
    /// </summary>
    /// <remarks>
    /// X is half of Z, Y is constant
    /// </remarks>
    /// <param name="size">Desired size float</param>
    public override void SetModelSize(float size)
    {
        if (size > 0)
        {
            transform.localScale = new Vector3(size / 2f, 1f, size);
        } else
        {
            transform.localScale = Vector3.one;
        }
    }
}
