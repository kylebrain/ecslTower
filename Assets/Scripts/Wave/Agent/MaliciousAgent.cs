using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaliciousAgent : Agent {

    /*-----------public override function-----------*/
    /// <summary>
    /// Specific action for this type of Agent
    /// </summary>
    public override void DestinationAction()
    {
        Health.health -= scoreMod;
    }

    private void Start()
    {
        if (LevelLookup.markMalicious)
        {
            GetComponent<Renderer>().material = Resources.Load<Material>("Agent/MarkAgent");
            SetColor(Attribute.Color);
        }
    }
}
