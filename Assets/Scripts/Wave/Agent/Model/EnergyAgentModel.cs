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
        Vector3 newSize = Vector3.one * size * 0.5f;
        if(size < 0)
        {
            newSize = new Vector3(1f, 0.5f, 1f);
        }
        transform.localScale = newSize;
    }
}
