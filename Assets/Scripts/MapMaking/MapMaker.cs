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

    /// <summary>
    /// If the map can be edited and interacted with
    /// </summary>
    public bool enableMapEditing = false;

    /// <summary>
    /// The GameObject which holds the UI aspects of the MapMaker
    /// </summary>
    public GameObject mapMakerDisplay;

    /// <summary>
    /// If the path can be edited
    /// </summary>
    /// <remarks>
    /// Determines if you can edit the Sources/Sink vs. the Path
    /// </remarks>
    [HideInInspector]
    public bool enablePathEditing = false;

    /// <summary>
    /// Number of AgentPaths make when Make is called
    /// </summary>
    public int makePerWave = 100;

    /// <summary>
    /// The map that is being edited
    /// </summary>
    [HideInInspector]
    public Map mapToEdit;

    #endregion

    #region private variables

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

    /// <summary>
    /// Prevents the Update function from being called more than once in Edit mode
    /// </summary>
    private bool ranOnEdit = false;

    #endregion

    #region Start and Update

    protected override void DerivedStart()
    {
        mapToEdit = FindMap();
    }

    /// <summary>
    /// Call the neccessary functions for operation
    /// </summary>
    private void Update()
    {
        //In Edit mode, loads the level from a file and applies it to the Prefab
            //Still need to click Apply to apply changes
        if (Application.isEditor && !Application.isPlaying && !ranOnEdit)
        {
            DerivedStart();
            ranOnEdit = true;
            if (mapToEdit == null)
            {
                Debug.LogError("Couldn't find map!");
                return;
            }
            if (mapToEdit.loadLevel.levelName == "DEFAULT_LEVEL")
            {
                return;
            }
            Load(mapToEdit);
            return;
        }

        //above should be run in inspector, place everything below it

        //enables toggling of the mapMakerDisplay display
        if(enableMapEditing != mapMakerDisplay.activeSelf)
        {
            mapMakerDisplay.SetActive(enableMapEditing);
        }

        if (enablePathEditing)
        {
            DrawArrowIfValid();
            SelectNodeOnClick();
            RemoveArrowOnClick();
        }
    }

    #endregion

    #region Level

    /// <summary>
    /// Allows Map to be cleared without reference
    /// </summary>
    public void ClearMap()
    {
        Clear(mapToEdit);
    }

    /// <summary>
    /// Allows Map to be saved without reference
    /// </summary>
    public void SaveMap()
    {
        Save(mapToEdit);
    }

    /// <summary>
    /// Clears both the wavePathList and the file of the Level
    /// </summary>
    /// <param name="map">Map to be cleared</param>
    private void Clear(Map map)
    {
        wavePathList = new List<WavePath>();
        DeleteArrowContainer(arrowContainer);
        map.loadLevel.DeleteLevel();
    }

    /// <summary>
    /// Saves the current wavePathList to the Level which will write to file
    /// </summary>
    /// <param name="map">Map to be saved</param>
    private void Save(Map map)
    {
        wavePathList = arrowContainer.ToWavePaths();
        if (wavePathList != null && wavePathList.Count > 0)
        {
            map.loadLevel.SetLevel(wavePathList, arrowContainer.startAreas, arrowContainer.endAreas);
            map.loadLevel.SaveLevel();
        }
        else
        {
            Debug.LogError("Path list has nothing in it");
        }
    }

    /// <summary>
    /// Loads the Level from file, can ultimately just read from the Level in the inspector
    /// </summary>
    /// <param name="map">Map to be loaded</param>
    private void Load(Map map)
    {
        SerializableLevel tempLevel = map.loadLevel.LoadLevel();
        if (tempLevel == null)
        {
            return;
        }
        List<SerializableWavePath> tempWavePathList = tempLevel.wavePaths;
        List<SerializableEndArea> tempEndAreaList = tempLevel.endAreas;
        if (tempWavePathList != null && tempEndAreaList != null)
        {
            map.loadLevel.SetLevel(tempLevel);
        }
        else
        {
            Debug.LogError("Loading failed!");
        }
    }

    #endregion

    #region arrowContainer

    /// <summary>
    /// Sets all the Nodes occupied by the ArrowContainer to empty, clears Arrows and EndAreas from the Container
    /// </summary>
    /// <param name="arrowContainer">ArrowContainer to be deleted</param>
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
    protected override void HandleEndArea(EndArea endArea, SerializableEndArea area) { }

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
                    Debug.LogWarning("Node" + currentNode.Coordinate + " cannot be placed here!", currentNode);
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
                if (SetPathSegment(currStart, currEnd, drawArrow))
                {
                    currStart = currEnd = null;
                    drawArrow = null;
                }
            }

        }
    }

    #endregion

}
