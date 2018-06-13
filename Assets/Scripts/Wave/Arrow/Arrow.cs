using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual and structure representation of the WavePath creation
/// </summary>
public class Arrow : MonoBehaviour
{

    /*-----------public variables-----------*/
    private float baseRadius = 0.1f;
    private float widthScale = 0.01f;
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
        Vector3 midpoint = Vector3.Lerp(start, end, 0.5f) + (Vector3.up * overlay) + 0.5f * baseRadius * Vector3.Normalize(end - start);
        float angle = Mathf.Atan2(end.z - start.z, end.x - start.x) * Mathf.Rad2Deg;
        float length = Vector3.Magnitude(start - end) / 2f;
        float width = baseRadius + widthScale * length;

        transform.position = midpoint;
        transform.eulerAngles = new Vector3(0f, angle, 90f);
        transform.localScale = new Vector3(width, length, width);
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(width, length));
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

    //returns a x by 1 grid area that represents the arrow

    public Vector2Int GetCardinality()
    {
        return new Vector2Int(0,0);
    }

    /// <summary>
    /// Prints the origin and destination Nodes when outputing the Arrow object
    /// </summary>
    /// <returns>The string to be outputted</returns>
    public override string ToString()
    {
        return base.ToString() + origin.Coordinate + "->" + destination.Coordinate;
    }

    public void KillArrrow()
    {
        Destroy(gameObject);
    }
}
