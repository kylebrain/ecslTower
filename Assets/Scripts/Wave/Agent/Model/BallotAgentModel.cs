using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Legacy Model
/// </summary>
/// <seealso cref="MoneyAgentModel"/>
public class BallotAgentModel : AgentModel {

    public override void SetModelColor(Color color)
    {
        Renderer rend = transform.Find("Border").GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }

    public override void SetModelSize(float size)
    {
        if (size < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1 / 1.5f);
        }
        else
        {
            transform.localScale = new Vector3(0.5f * size, 1f, 0.5f * size);
        }
    }
}
