using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Legacy Model
/// </summary>
/// <seealso cref="MoneyAgentModel"/>
public class BallotAgentModel : SquareModel {

    public override void SetModelColor(Color color)
    {
        Renderer rend = transform.Find("Border").GetComponent<Renderer>();
        rend.material.SetColor("_Color", color);
    }
}
