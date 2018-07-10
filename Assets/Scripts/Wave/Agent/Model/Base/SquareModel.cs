using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for an already created model that is scaled equally in the X and Z axes
/// </summary>
public abstract class SquareModel : AgentModel {

    /// <summary>
    /// Prefab scaled square based
    /// </summary>
    /// <remarks>
    /// X and Z are scaled together, Y stays constant
    /// </remarks>
    /// <param name="size">Desired size float</param>
    public override void SetModelSize(float size)
    {
        if (size > 0)
        {
            transform.localScale = new Vector3(0.5f * size, 1f, 0.5f * size);
        }
        else
        {
            //scales to a cube when invalid size is passed
            transform.localScale = new Vector3(1f, 1f, 1 / 1.5f);
        }
    }
}
