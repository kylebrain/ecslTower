using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for a model that is a cube and must be scaled to a rectangle
/// </summary>
public abstract class RectangularModel : AgentModel
{

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
            transform.localScale = Vector3.one;
        }
    }
}
