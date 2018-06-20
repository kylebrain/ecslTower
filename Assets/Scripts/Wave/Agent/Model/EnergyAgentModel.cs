using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

/// <summary>
/// Handles the Model for the Energy prefab
/// </summary>
public class EnergyAgentModel : AgentModel {

    /// <summary>
    /// Sets the material color and the lightning color
    /// </summary>
    /// <param name="color">Desired color</param>
    public override void SetModelColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        Color currentColor = rend.material.GetColor("_Color");
        color.a = currentColor.a; //keeps the alpha from the original material

        rend.material.SetColor("_Color", color);
        LightningMeshSurfaceScript energy = transform.Find("Energy").GetComponent<LightningMeshSurfaceScript>();
        energy.GlowTintColor = color;
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
        }
        else
        {
            //perfect cube if size is not recognized
            transform.localScale = Vector3.one;
        }
    }
}
