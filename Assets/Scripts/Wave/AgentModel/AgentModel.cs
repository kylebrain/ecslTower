using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentModel : MonoBehaviour {

    public abstract void SetColor(Color color);
    public abstract void SetSize(Vector3 size); //change to float later
}
