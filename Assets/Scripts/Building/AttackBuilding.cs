using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuilding : Building {

    private void Start() {
        radius = 2;
        initLineRenderer();


        placeOnMap( new GridArea(new Vector2Int(3, 2), 3, 4) );
    }


    void Update () {
        showRadius();
        
    }

    private void FixedUpdate() {
        updateAction();
    }

    public override void updateAction() {

    }
}
