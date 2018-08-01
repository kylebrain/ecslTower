using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PolygonVertex {

    public int selection;
    public Vector2 position;
    public Button left;
    public Button right;

    public PolygonVertex () : this(-1, Vector2.zero) { }

    public PolygonVertex(int _selection, Vector2 _position, Button _left = null, Button _right = null)
    {
        selection = _selection;
        position = _position;
        left = _left;
        right = _right;
    }

    public void Enable(bool enabled)
    {
        left.interactable = enabled;
        right.interactable = enabled;
    }

    public override bool Equals(object obj)
    {
        if(obj.GetType() != GetType())
        {
            return false;
        } else
        {
            PolygonVertex vertex = (PolygonVertex)obj;
            return position == vertex.position && selection == vertex.selection && left == vertex.left && right == vertex.right;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
