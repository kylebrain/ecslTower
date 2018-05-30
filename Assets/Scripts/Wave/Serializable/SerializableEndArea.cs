using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEndArea {
    public bool Sink;
    public Coordinate bottomLeft;
    public int width;
    public int height;
    public SerializableEndArea(GridArea gridArea, bool sink)
    {
        bottomLeft = new Coordinate(gridArea.bottomLeft.x, gridArea.bottomLeft.y);
        width = gridArea.width;
        height = gridArea.height;
        Sink = sink;
    }
}
