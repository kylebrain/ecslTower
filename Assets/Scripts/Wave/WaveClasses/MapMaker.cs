using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Manages the waves, allows for building of the WavePaths and Waves
/// </summary>
[ExecuteInEditMode]
public class MapMaker : MapDisplay
{

    #region public variables
    /*-----------public variables-----------*/
    public bool enableMapEditing = false;

    public GameObject levelCreation;

    [HideInInspector]
    public bool enablePathEditing = false;
    
    /// <summary>
    /// Number of AgentPaths make when Make is called
    /// </summary>
    public int makePerWave = 100;

    [HideInInspector]
    public Map mapToEdit;

    #endregion

    #region private variables
    /*-----------private variables-----------*/
    /// <summary>
    /// Used for Arrow drawing, where the base of the Arrow will lay
    /// </summary>
    private Node currStart = null;
    /// <summary>
    /// Used for Arrow drawing, where the tip of the Arrow will lay
    /// </summary>
    private Node currEnd = null;
    /// <summary>
    /// The Arrow that is currently being drawn
    /// </summary>
    private Arrow drawArrow = null;

    #endregion

    #region Start and Update

    protected override void DerivedStart()
    {
        GetWorldGrid();
        mapToEdit = FindMap();
    }

    /// <summary>
    /// Call the neccessary functions for operation
    /// </summary>
    private void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            DerivedStart();
            if(mapToEdit == null)
            {
                Debug.LogError("Couldn't find map!");
            }
            Load(mapToEdit);
            return;
        }


            if (!levelCreation.activeSelf && enableMapEditing)
        {
            levelCreation.SetActive(true);
        }

        if (!enableMapEditing)
        {
            if (levelCreation.activeSelf)
            {
                levelCreation.SetActive(false);
            }
            return;
        }


        
        if (enablePathEditing)
        {
            DrawArrowIfValid();
            SelectNodeOnClick();
            RemoveArrowOnClick();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (PushPath())
                {
                    Debug.Log("Path pushed!");
                }
                else
                {
                    Debug.LogError("Invalid path!");
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Save(mapToEdit);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                Load(mapToEdit);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Clear(mapToEdit);
            }
        }
    }

    #endregion

    #region Level

    /// <summary>
    /// Clears both the wavePathList and the file of the Level
    /// </summary>
    private void Clear(Map map)
    {
        Debug.Log("Cleared!");
        wavePathList = new List<WavePath>();
        DeleteArrowContainer(arrowContainer);
        map.loadLevel.DeleteLevel();
    }

    /// <summary>
    /// Saves the current wavePathList to the Level which will write to file
    /// </summary>
    private void Save(Map map)
    {
        if (wavePathList != null && wavePathList.Count > 0)
        {
            map.loadLevel.SetLevel(wavePathList, arrowContainer.startAreas, arrowContainer.endAreas);
            map.loadLevel.SaveLevel();

            foreach (WavePath path in wavePathList)
            {
                Debug.Log("Saved: " + path);
            }
        }
        else
        {
            Debug.LogError("Path list has nothing in it");
        }
    }

    #endregion

    #region arrowContainer

    private void DeleteArrowContainer(ArrowContainer arrowContainer)
    {
        foreach (Stack<Arrow> stacks in arrowContainer.arrowStacks)
        {
            foreach (Arrow arrow in stacks)
            {
                SetNodeOccupation(arrow, Node.nodeStates.empty);
                Destroy(arrow.gameObject);
            }
        }
        arrowContainer.arrowStacks = new List<Stack<Arrow>>();
        arrowContainer.startAreas = new List<GridArea>();
        arrowContainer.endAreas = new List<GridArea>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EndArea"))
        {
            Destroy(obj);
        }
    }

    protected override void HandleEndArea(EndArea endArea, SerializableEndArea area)
    {
        
    }

    #endregion

    #region Arrow

    /// <summary>
    /// On RightClick, the Arrow selected and every Arrow after it is removed
    /// </summary>
    private void RemoveArrowOnClick()
    {
        if (Input.GetMouseButtonDown(1))
        {

            Node NodePointedAt = worldGrid.getRaycastNode();
            if (NodePointedAt == null) //exit function if no node is selected
            {
                return;
            }
            List<Arrow> deleteArrows = arrowContainer.RemoveArrows(NodePointedAt);
            if (deleteArrows.Count == 0)
            {
                return;
            }
            foreach (Arrow arrow in deleteArrows)
            {
                SetNodeOccupation(arrow, Node.nodeStates.empty);
                arrow.KillArrrow();
            }
        }
    }

    /// <summary>
    /// Will update the Arrow about to be placed to be drawn with the mouse
    /// </summary>
    private void DrawArrowIfValid()
    {
        if (currStart != null && drawArrow != null && currEnd == null)
        {
            Node nodePointedAt = worldGrid.getRaycastNode();
            if (nodePointedAt != null)
            {
                drawArrow.DrawArrow(currStart.transform.position, GetCardinalNode(nodePointedAt, currStart).transform.position, arrowOffset);
            }
        }
    }


    #endregion

    #region Node and Arrow Creation

    /// <summary>
    /// Returns a Node that is perpendicular to the start Node
    /// </summary>
    /// <param name="target">The Node which the mouse is currently over</param>
    /// <param name="origin">The origin of the Arrow which needs to be placed</param>
    /// <returns></returns>
    private Node GetCardinalNode(Node target, Node origin)
    {
        int distanceY = Mathf.Abs(target.Coordinate.y - origin.Coordinate.y);
        int distanceX = Mathf.Abs(target.Coordinate.x - origin.Coordinate.x);

        if (distanceX >= distanceY)
        {
            return worldGrid.getAt(target.Coordinate.x, origin.Coordinate.y);
        }
        else
        {
            return worldGrid.getAt(origin.Coordinate.x, target.Coordinate.y);
        }
    }

    /// <summary>
    /// Handles the Node selection on LeftClick to provide currStart and currEnd valid Nodes to be used by other functions
    /// </summary>
    private void SelectNodeOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Node currentNode = worldGrid.getRaycastNode();
            if (currentNode == null) //exit function if no node is selected
            {
                return;
            }

            if (currStart == null) //if there is no selected start node
            {
                if (arrowContainer.IsVaildNode(currentNode))
                {
                    currStart = currentNode;
                    //Debug.Log("Start Point set to: " + currStart.name);
                    drawArrow = Instantiate(arrowPrefab, transform) as Arrow;
                    drawArrow.DrawArrow(currStart.transform.position, worldGrid.getRaycastNode().transform.position, arrowOffset);
                }
                else
                {
                    Debug.LogError("Node" + currentNode.Coordinate + " cannot be placed here!");
                }
            }

            else //if start node has been selected, select the end node
            {
                currEnd = GetCardinalNode(currentNode, currStart);
                if (currEnd == currStart || currEnd.Occupied == Node.nodeStates.navigation) //end and start cannot be the same
                {
                    Debug.LogError("Node" + currentNode.Coordinate + " is occupied!");
                    currEnd = null;
                }
                else
                {
                    //Debug.Log("End Point set to: " + currEnd.name);
                }
            }

            if (currStart != null && currEnd != null) //if both the start and end exist, set the path segment
            {
                if(SetPathSegment(currStart, currEnd, drawArrow))
                {
                    currStart = currEnd = null;
                    drawArrow = null;
                }
            }

        }
    }

    #endregion

    #region Path Creation

    private bool PushPath()
    {
        wavePathList = arrowContainer.ToWavePaths();
        return true;
    }

    #endregion

}
