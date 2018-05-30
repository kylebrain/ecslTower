using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAreaButton : GameButton
{
    public EndArea endAreaPrefab;
    public bool sink; //sink if true, source if false

    public override void PerformAction()
    {
        EndArea area = Instantiate(endAreaPrefab, transform);
        area.endSetting = sink ? endOptions.sink : endOptions.source;
    }
}