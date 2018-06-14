using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))] //the collider must be here and not on the children
public abstract class AgentModel : MonoBehaviour {
    public abstract void SetColor(Color color);
    public abstract void SetSize(float size);
}
