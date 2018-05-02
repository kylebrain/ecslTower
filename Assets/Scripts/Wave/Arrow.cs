using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual and structure representation of the WavePath creation
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Arrow : MonoBehaviour
{

    /*-----------public variables-----------*/
    /// <summary>
    /// What percentage the Arrow head is of the arrow
    /// </summary>
    /// <remarks>
    /// Default works fine
    /// </remarks>
    public float headLength = 10f;
    /// <summary>
    /// The Node the base of the Arrow is attached (get only)
    /// </summary>
    public Node Origin
    {
        get
        {
            return origin;
        }
    }
    /// <summary>
    /// The Node the tip of the Arrow is attached (get only)
    /// </summary>
    public Node Destination
    {
        get
        {
            return destination;
        }
    }


    /*-----------private variables-----------*/
    private Node origin;
    private Node destination;


    /*-----------public functions-----------*/
    /// <summary>
    /// Will draw the Arrow dynamically
    /// </summary>
    /// <param name="start">Where the base of the arrow should lay</param>
    /// <param name="end">Where the tip of the arrow should lay</param>
    /// <param name="overlay">How much in the Y direction the arrow is above the passed values</param>
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

    /// <summary>
    /// Places the arrow and sets its values to be used as a data structure
    /// </summary>
    /// <param name="start">Where the base of the arrow should lay</param>
    /// <param name="end">Where the tip of the arrow should lay</param>
    /// <param name="overlay">How much in the Y direction the arrow is above the passed values</param>
    public void PlaceArrow(Node start, Node end, float overlay)
    {
        DrawArrow(start.transform.position, end.transform.position, overlay);
        origin = start;
        destination = end;
    }

    /// <summary>
    /// Prints the origin and destination Nodes when outputing the Arrow object
    /// </summary>
    /// <returns>The string to be outputted</returns>
    public override string ToString()
    {
        return base.ToString() + origin.Coordinate + "->" + destination.Coordinate;
    }
}
