using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAgentModel : AgentModel {

    public override void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }

    public override void SetSize(float size)
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
