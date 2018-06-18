using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

public class EnergyAgentModel : AgentModel {

    public override void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        Color currentColor = rend.material.GetColor("_Color");
        color.a = currentColor.a;

        rend.material.SetColor("_Color", color);
        LightningMeshSurfaceScript energy = transform.Find("Energy").GetComponent<LightningMeshSurfaceScript>();
        energy.GlowTintColor = color;
    }

    public override void SetSize(float size)
    {
        if (size > 0)
        {
            transform.localScale = new Vector3(size / 2f, 1f, size);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}
