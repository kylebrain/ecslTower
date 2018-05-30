using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum endOptions { source, sink };
public class EndArea : MonoBehaviour {

    //press a button to select either a source or sink
    //click to start the origin point and drag to expand the area
    //gridArea will be created once the area is selected

    public endOptions endSetting = endOptions.source;
    public GridArea area;
    private GameObject colorPlaceholder = null;
    private Node origin = null;
    private Node destination = null;
    private WorldGrid worldGrid; //remove if a solution can be found

    private void Start()
    {
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }

        //endSetting must be set first using some function
        if (endSetting == endOptions.source)
        {
            colorPlaceholder = Resources.Load<GameObject>("EndArea/SourceCube");
        } else if (endSetting == endOptions.sink)
        {
            colorPlaceholder = Resources.Load<GameObject>("EndArea/SinkCube");
        }

        if(colorPlaceholder == null)
        {
            Debug.LogError("Cannot find EndArea placeholder prefab in the Resources folder!");
            return;
        }

    }

    private void Update()
    {
        OnLeftMouseDown();
    }

    private void OnLeftMouseDown()
    {
        if(origin != null && destination != null)
        {
            return; //area had been placed
        }
        if (Input.GetMouseButtonDown(0))
        {
            if(origin == null)
            {
                //set origin if it doesn't exist
                origin = worldGrid.getRaycastNode();
            } else
            {
                destination = worldGrid.getRaycastNode();
            }

            if (origin != null && destination != null)
            {
                PlaceEndArea();
            }
        }
    }

    private void PlaceEndArea()
    {
        //destination = worldGrid.getRaycastNode();
        if (origin == null || destination == null)
        {
            Debug.LogError("Must have valid start and end Nodes!");
            return;
        }

        Vector2Int bottomleft = new Vector2Int(Mathf.Min(origin.Coordinate.x, destination.Coordinate.x), Mathf.Min(origin.Coordinate.y, destination.Coordinate.y));
        Vector2Int topRight = new Vector2Int(Mathf.Max(origin.Coordinate.x, destination.Coordinate.x), Mathf.Max(origin.Coordinate.y, destination.Coordinate.y));

        area.bottomLeft = bottomleft;
        area.width = topRight.x - bottomleft.x + 1;
        area.height = topRight.y - bottomleft.y + 1;


        /*if(Vector3.SqrMagnitude(origin.transform.position) < Vector3.SqrMagnitude(destination.transform.position))
        {
            //if the origin is closer to the bottom left
            area.bottomLeft = origin.Coordinate;
            area.width = Mathf.Abs(destination.Coordinate.x - origin.Coordinate.x) + 1;
            area.height = Mathf.Abs(destination.Coordinate.y - origin.Coordinate.y) + 1;
        } else
        {
            area.bottomLeft = destination.Coordinate;
            area.width = Mathf.Abs(origin.Coordinate.x - destination.Coordinate.x) + 1;
            area.height = Mathf.Abs(origin.Coordinate.y - destination.Coordinate.y) + 1;
        }*/
        if(area.width <= 0 || area.height <= 0)
        {
            Debug.LogError("Invalid gridArea created!");
            return;
        }
        Debug.Log("Created area: " + area);
        MarkArea();
    }

    private void MarkArea()
    {
        if(area.height == 0 || area.width == 0)
        {
            Debug.LogError("Must have a valid gridArea!");
            return;
        }
        for (int i = area.bottomLeft.x; i < area.bottomLeft.x + area.width; i++)
        {
            for (int j = area.bottomLeft.y; j < area.bottomLeft.y + area.height; j++)
            {
                GameObject newMarker = Instantiate(colorPlaceholder, worldGrid.getAt(i, j).transform.position, Quaternion.identity) as GameObject;
                newMarker.transform.parent = transform;
            }
        }
        //UpdateArea();
    }

    private void UpdateArea()
    {
        if (area.height == 0 || area.width == 0)
        {
            Debug.LogError("Must have a valid gridArea!");
            return;
        }
        for(int i = 0; i < transform.childCount; i++)
        {
            if(area.Contains(new Vector2Int((int)transform.GetChild(i).position.x, (int)transform.GetChild(i).position.y))){
                continue;
            }
            Destroy(transform.GetChild(i).gameObject);
            i--;
        }
    }
}
