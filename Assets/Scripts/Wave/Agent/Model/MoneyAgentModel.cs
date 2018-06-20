using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Model for the MoneyAgent prefab
/// </summary>
public class MoneyAgentModel : AgentModel {

    /// <summary>
    /// Sets the Border color through the material
    /// </summary>
    /// <param name="color">Desired color</param>
    public override void SetModelColor(Color color)
    {
        Renderer rend = transform.Find("Border").GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }

    /// <summary>
    /// Prefab scaled square based
    /// </summary>
    /// <remarks>
    /// X and Z are scaled together, Y stays constant
    /// </remarks>
    /// <param name="size">Desired size float</param>
    public override void SetModelSize(float size)
    {
        if (size < 0)
        {
            //scales to a cube when invalid size is passed
            transform.localScale = new Vector3(1f, 1f, 1 / 1.5f);
        }
        else
        {
            transform.localScale = new Vector3(0.5f * size, 1f, 0.5f * size);
        }
    }
}
