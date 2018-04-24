using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuilding : Building {

    private void Start() {
        radius = 5;
        initLineRenderer();
    }


    void Update () {
        showRadius();
        
    }

    public override void updateAction() {

    }
}
