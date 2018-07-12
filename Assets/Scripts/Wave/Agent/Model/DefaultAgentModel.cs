using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default template Model for new maps
/// </summary>
public class DefaultAgentModel : RectangularModel {

    /// <summary>
    /// Sets the color
    /// </summary>
    /// <param name="color">Desired color</param>
    public override void SetModelColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }
}
