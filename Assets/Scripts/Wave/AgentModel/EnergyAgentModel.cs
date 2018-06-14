using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

public class EnergyAgentModel : AgentModel {

    public override void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
        LightningMeshSurfaceScript energy = transform.Find("Energy").GetComponent<LightningMeshSurfaceScript>();
        energy.GlowTintColor = color;
    }

    public override void SetSize(Vector3 size)
    {
        Vector3 newSize = Vector3.one * size.x;
        transform.localScale = newSize;
    }
}
