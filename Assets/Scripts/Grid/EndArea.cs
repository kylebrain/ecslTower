using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum endOptions { source, sink };
public class EndArea : MonoBehaviour
{
    //make sure only one endarea is being created at a time

    //press a button to select either a source or sink
    //click to start the origin point and drag to expand the area
    //gridArea will be created once the area is selected

    public endOptions endSetting = endOptions.source;
    public GridArea area;
    private GameObject colorPlaceholder = null;
    private Node Origin = null;
    private Node Destination = null;
    private WorldGrid worldGrid; //remove if a solution can be found
    private MapMaker waveManager; //remove if a solution can be found

    private void Awake()
    {
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }

        GameObject managerObject = GameObject.FindWithTag("MapMaker");
        if(managerObject == null)
        {
            //EndArea is a display area, update when a DisplayArea is created
            return;
        }
        waveManager = managerObject.GetComponent<MapMaker>();
        if (waveManager == null)
        {
            Debug.LogError("Could not find MapMaker object in the scene. Either the tag was changed or the object is missing.");
        }

        //SetColor();

    }

    private void Update()
    {
        OnLeftMouseDown();
        OnRightMouseDown();

    }

    public void SetColor()
    {
        if (endSetting == endOptions.source)
        {
            colorPlaceholder = Resources.Load<GameObject>("EndArea/SourceCube");
        }
        else if (endSetting == endOptions.sink)
        {
            colorPlaceholder = Resources.Load<GameObject>("EndArea/SinkCube");
        }

        if (colorPlaceholder == null)
        {
            Debug.LogError("Cannot find EndArea placeholder prefab in the Resources folder!");
            return;
        }
    }

    private void OnRightMouseDown()
    {
        if (Input.GetMouseButtonDown(1) && !waveManager.enablePathEditing)
        {
            Node currentNode;
            if ((currentNode = worldGrid.getRaycastNode()) == null)
            {
                return;
            }
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EndArea"))
            {
                EndArea currentEndArea = obj.GetComponent<EndArea>();
                if(currentEndArea == null)
                {
                    Debug.LogError("EndArea tagged object does not have the script!");
                    continue;
                }
                
                if (currentEndArea.area.Contains(currentNode.Coordinate))
                {
                    waveManager.arrowContainer.startAreas.Remove(currentEndArea.area);
                    waveManager.arrowContainer.endAreas.Remove(currentEndArea.area);
                    Destroy(currentEndArea.gameObject);
                }
            }
        }
    }

    private void OnLeftMouseDown()
    {
        if (Origin != null && Destination != null)
        {
            return; //area had been placed
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Origin == null)
            {
                //set origin if it doesn't exist
                Origin = worldGrid.getRaycastNode();
            }
            else
            {
                Destination = worldGrid.getRaycastNode();
            }

            if (Origin != null && Destination != null)
            {
                PlaceEndArea(Origin, Destination);
            }
        }
    }

    public void PlaceEndArea(Node origin, Node destination)
    {
        Origin = origin;
        Destination = destination;
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

        if (area.width <= 0 || area.height <= 0)
        {
            Debug.LogError("Invalid gridArea created!");
            return;
        }
        MarkArea(area);
    }

    private void MarkArea(GridArea area)
    {
        if (area.height <= 0 || area.width <= 0)
        {
            Debug.LogError("Must have a valid gridArea!");
            return;
        }
        Vector3 pos = new Vector3(area.bottomLeft.x + (area.width - 1) / 2f, 0f, area.bottomLeft.y + (area.height - 1) / 2f);
        Vector3 scale = new Vector3(area.width, transform.localScale.y, area.height);
        GameObject newMarker = Instantiate(colorPlaceholder, pos, Quaternion.identity) as GameObject;
        newMarker.transform.parent = transform;
        newMarker.transform.localScale = scale;
        if (waveManager != null)
        {
            AddToArrowContainer();
        }
    }

    private void AddToArrowContainer()
    {
        if (area.height <= 0 || area.width <= 0)
        {
            Debug.LogError("Must have a valid gridArea!");
            return;
        }
        if(endSetting == endOptions.source)
        {
            waveManager.arrowContainer.startAreas.Add(area);
        } else if (endSetting == endOptions.sink)
        {
            waveManager.arrowContainer.endAreas.Add(area);
        }
    }

    //use later to dynamically display the area, update each time the destination Node changes
    private void UpdateArea()
    {
        if (area.height == 0 || area.width == 0)
        {
            Debug.LogError("Must have a valid gridArea!");
            return;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (area.Contains(new Vector2Int((int)transform.GetChild(i).position.x, (int)transform.GetChild(i).position.y)))
            {
                continue;
            }
            Destroy(transform.GetChild(i).gameObject);
            i--;
        }
    }
}
