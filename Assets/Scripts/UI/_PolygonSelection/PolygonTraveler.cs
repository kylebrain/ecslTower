using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonTraveler : MonoBehaviour
{

    public PolygonVertex currentVertex = null;
    public PolygonVertex destinationVertex = null;

    public bool Moving
    {
        get
        {
            return !currentVertex.Equals(destinationVertex);
        }
    }

    public int Selection
    {
        get
        {
            return currentVertex.selection;
        }
    }

    public float buffer = 2f;
    public float speed = 1f;


    private void Update()
    {
        if (destinationVertex != null)
        {
            Vector2 movement = (destinationVertex.position - currentVertex.position) * speed * Time.deltaTime;
            if (Vector2.SqrMagnitude(new Vector2(transform.localPosition.x, transform.localPosition.y) - destinationVertex.position) < buffer)
            {
                
                currentVertex = destinationVertex;
                currentVertex.Enable(true);
            }
            else
            {
                transform.localPosition += new Vector3(movement.x, movement.y);
            }
        }

    }

    public void SetCurrentVertex(PolygonVertex vertex)
    {

        currentVertex = vertex;
        transform.localPosition = currentVertex.position;
        destinationVertex = vertex;
    }
}
