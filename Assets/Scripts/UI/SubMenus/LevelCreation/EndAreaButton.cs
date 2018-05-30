using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAreaButton : GameButton
{
    public EndArea endAreaPrefab;
    public Transform markerHolder;
    public bool sink; //sink if true, source if false

    public override void PerformAction()
    {
        SpawnEndArea(sink);
    }

    public void SpawnEndArea (bool sink)
    {
        EndArea area = Instantiate(endAreaPrefab, markerHolder);
        area.endSetting = sink ? endOptions.sink : endOptions.source;
    }
}