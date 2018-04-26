using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Arrow : MonoBehaviour
{

    public float headLength = 10f;
    public Node Origin
    {
        get
        {
            return origin;
        }
    }
    public Node Destination
    {
        get
        {
            return destination;
        }
    }
    private Node origin;
    private Node destination;

    public void DrawArrow(Vector3 start, Vector3 end, float overlay)
    {
        start += new Vector3(0, overlay, 0);
        end += new Vector3(0, overlay, 0);
        Vector3 connectionPoint = Vector3.Lerp(start, end, (100f - headLength) / 100f);

        LineRenderer arrowhead = transform.Find("Arrowhead").GetComponent<LineRenderer>();

        arrowhead.material.color = Color.grey;

        if (arrowhead == null)
        {
            Debug.LogError("Cannot find child arrowhead,\nPerhaps it was moved?");
        }
        else
        {
            arrowhead.SetPosition(0, connectionPoint);
            arrowhead.SetPosition(1, end);
        }
        LineRenderer straight = GetComponent<LineRenderer>();

        straight.material.color = Color.grey;

        straight.SetPosition(0, start);
        straight.SetPosition(1, connectionPoint);
    }

    public void PlaceArrow(Node start, Node end, float overlay)
    {
        DrawArrow(start.transform.position, end.transform.position, overlay);
        origin = start;
        destination = end;
    }

    public override string ToString()
    {
        return base.ToString() + origin.Coordinate + "->" + destination.Coordinate;
    }
}
