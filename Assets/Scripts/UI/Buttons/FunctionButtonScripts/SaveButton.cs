using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : GameButton {

    private MapMaker mapMaker;

    private void Start()
    {
        mapMaker = GameObject.FindGameObjectWithTag("MapMaker").GetComponent<MapMaker>();
    }

    public override void PerformAction()
    {
        mapMaker.SaveMap();
    }
}
