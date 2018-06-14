using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAgentModel : AgentModel {

    public override void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }

    public override void SetSize(Vector3 size)
    {
        transform.localScale = size;
    }
}
