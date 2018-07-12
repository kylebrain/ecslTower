using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Model for the MoneyAgent prefab
/// </summary>
public class MoneyAgentModel : SquareModel {

    /// <summary>
    /// Sets the Border color through the material
    /// </summary>
    /// <param name="color">Desired color</param>
    public override void SetModelColor(Color color)
    {
        Renderer rend = transform.Find("Border").GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }
}
