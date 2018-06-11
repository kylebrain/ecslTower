using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButton : GameButton {

    private MapMaker mapMaker;
    private KeyCode confirmKey;
    private bool waitingConfirm = false;

    private void Start()
    {
        mapMaker = GameObject.FindGameObjectWithTag("MapMaker").GetComponent<MapMaker>();
    }

    public override void PerformAction()
    {
        confirmKey = KeyCode.A + Random.Range(0, 26);
        waitingConfirm = true;
        Debug.Log("Press \"" + confirmKey + "\" to confirm clear. Any other key to cancel.");
    }

    private void Update()
    {
        if (waitingConfirm)
        {
            if (Input.GetKeyDown(confirmKey))
            {
                waitingConfirm = false;
                mapMaker.ClearMap();
            } else if (Input.anyKeyDown)
            {
                waitingConfirm = false;
                Debug.Log("Clear canceled!");
            }
        }
    }

}
