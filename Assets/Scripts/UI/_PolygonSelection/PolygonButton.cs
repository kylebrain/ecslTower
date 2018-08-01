using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonButton : Button {

    // as seen looking from outside the line
    public PolygonVertex left;
    public PolygonVertex right;

    public PolygonSelection selection;

    protected override void Start()
    {
        base.Start();
        selection = transform.parent.GetComponent<PolygonSelection>();
        if (selection == null)
        {
            Debug.LogError("PolygonButton must be a child of a PolygonSelection!");
            return;
        }
    }

    public void ButtonClicked()
    {
        //PolygonVertex nextVertex = (vertex == left ? right : left);
        selection.MoveTraveler(this);
    }
}
