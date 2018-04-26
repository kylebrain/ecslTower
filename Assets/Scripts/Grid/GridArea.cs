using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores an area of the worldGrid. Holds the bottom left corner, a width, and a height
/// </summary>
public struct GridArea{
    public GridArea(Vector2Int bottom_left, int w, int h) {
        bottomLeft = bottom_left;
        width = w;
        height = h;
    }


    public Vector2Int bottomLeft;
    public int width;
    public int height;

    public override string ToString() {
        return base.ToString() + " bottomLeft: " + bottomLeft + " width: " + width + " height: " + height;
    }
}
