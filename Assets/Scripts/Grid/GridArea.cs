using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores an area of the worldGrid. Holds the bottom left corner, a width, and a height
/// </summary>
[System.Serializable]
public class GridArea : System.Object {
    public GridArea(Vector2Int bottom_left, int w, int h) {
        bottomLeft = bottom_left;
        width = w;
        height = h;
    }

    public GridArea(SerializableEndArea serializableEndArea)
    {
        bottomLeft = new Vector2Int(serializableEndArea.bottomLeft.x, serializableEndArea.bottomLeft.y);
        width = serializableEndArea.width;
        height = serializableEndArea.height;
    }

    public GridArea(){}

    public Vector2Int bottomLeft;
    public int width;
    public int height;


    public bool Contains(Vector2Int coordinate) {
        return (coordinate.x >= bottomLeft.x && coordinate.x < bottomLeft.x + width) && (coordinate.y >= bottomLeft.y && coordinate.y < bottomLeft.y + height);
    }

    public override string ToString() {
        return base.ToString() + " bottomLeft: " + bottomLeft + " width: " + width + " height: " + height;
    }
}
