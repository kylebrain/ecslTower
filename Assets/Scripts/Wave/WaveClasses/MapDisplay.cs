using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapDisplay : MonoBehaviour
{

    #region public variables

    public Map currentMap;

    /// <summary>
    /// How much the arrow will overlay on the grid
    /// </summary>
    /// <remarks>
    /// Default works fine
    /// </remarks>
    public float arrowOffset = 0.1f;

    public List<WavePath> WavePathList
    {
        get
        {
            return new List<WavePath>(wavePathList);
        }
    }

    /// <summary>
    /// Manatory prefab so an Arrow can be drawn and placed
    /// </summary>
    public Arrow arrowPrefab;

    public EndArea endAreaPrefab;

    /// <summary>
    /// Contains the Arrows and allows creation and deletion of the path of Arrows
    /// </summary>
    public ArrowContainer arrowContainer;

    public static bool mapLoaded = false;

    #endregion


    #region protected variables

    ///<summary>
    /// Reference to the WorldGrid object
    /// </summary>
    /// <remarks>
    /// Necessary for raycasting to Nodes
    /// </remarks>
    protected WorldGrid worldGrid;

    ///<summary>
    /// List of WavePaths recieved by the ArrowContainer or the SerializedWavePaths to be used by the Agents
    /// </summary>
    protected List<WavePath> wavePathList = new List<WavePath>();

    /// <summary>
    /// The Level that stores the current WavePaths in the inspector and the file
    /// </summary>
    private SerializableLevel currentLevel;

    #endregion

    private void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        GetWorldGrid();
        currentMap = FindMap();
        if (currentMap == null)
        {
            if (LevelLookup.levelName == "DEFAULT_VALUE") {
                Debug.LogError("Could not find map!");
                return;
            } else
            {
                currentMap = Resources.Load<Map>("Levels/" + LevelLookup.levelName);
                if (currentMap == null)
                {
                    Debug.LogError("LevelLookup does not have a valid level name!");
                    return;
                } else
                {
                    currentMap = Instantiate(currentMap);
                }

            }
        }

        currentMap.BuildWorldGrid(worldGrid);

        currentMap.SetLookup();

        currentLevel = currentMap.loadLevel;
        UseLevel(currentLevel);

        mapLoaded = true;

        DerivedStart();
    }

    protected virtual void DerivedStart() { }

    protected void GetWorldGrid()
    {
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }
    }

    protected Map FindMap()
    {
        GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
        if (mapObject == null)
        {
            //Debug.LogError("Cannot find an object with the Map tag!");
            return null;
        }
        Map map = mapObject.GetComponent<Map>();
        if (map == null)
        {
            //m_Debug.LogError("Object tagged Map does not have a Map script attached!");
            return null;
        }
        return map;
    }

    /// <summary>
    /// Populates wavePathList based on a List of SerializableWavePaths
    /// </summary>
    /// <param name="paths">List to be converted</param>
    protected void UseLevel(SerializableLevel level)
    {
        SetArrowContainerAreas(level.endAreas);
        wavePathList = new List<WavePath>();
        foreach (SerializableWavePath path in level.wavePaths)
        {
            wavePathList.Add(new WavePath(path, worldGrid));
        }

        DisplayPath(wavePathList);
    }

    protected void SetArrowContainerAreas(List<SerializableEndArea> endAreas)
    {
        Transform markerHolder = transform.Find("MarkerHolder").transform;
        if (markerHolder == null)
        {
            Debug.LogError("Cannot find marker holder! Perhaps it was moved or renamed?");
            return;
        }
        foreach (SerializableEndArea area in endAreas)
        {
            EndArea endArea = Instantiate(endAreaPrefab, markerHolder);
            endArea.endSetting = area.Sink ? endOptions.sink : endOptions.source;
            endArea.SetColor();
            GridArea tempArea = new GridArea(area);
            endArea.PlaceEndArea(worldGrid.getAt(tempArea.bottomLeft.x, tempArea.bottomLeft.y), worldGrid.getAt(tempArea.bottomLeft.x + tempArea.width - 1, tempArea.bottomLeft.y + tempArea.height - 1));
            HandleEndArea(endArea);
        }
    }

    protected virtual void HandleEndArea(EndArea endArea, SerializableEndArea area = null)
    {
        endArea.enabled = false;
    }

    /// <summary>
    /// When finished placing an Arrow, the Arrow will be placed and adhere to the Grid
    /// </summary>
    /// <param name="start">The Node in which the base of the Arrow is in</param>
    /// <param name="end">The Node in which the tip of the Arrow is in</param>
    /// <returns>success</returns>
    protected bool SetPathSegment(Node start, Node end, Arrow toSet = null)
    {

        if (start == null || end == null)
        {
            Debug.LogError("Passed invalid nodes to create segment!");
            return false;
        }
        if (toSet == null)
        {
            toSet = Instantiate(arrowPrefab, transform) as Arrow;
        }
        toSet.PlaceArrow(start, end, arrowOffset);
        arrowContainer.AddArrowToContainer(toSet);
        SetNodeOccupation(toSet, Node.nodeStates.navigation);

        return true;
    }

    /// <summary>
    /// Creates the visual Arrow path
    /// </summary>
    /// <param name="wavePaths">Will create the path based on this value</param>
    protected void DisplayPath(List<WavePath> wavePaths)
    {
        foreach (WavePath path in wavePaths)
        {
            WavePath temp = new WavePath(path);
            Node current = null;
            Node previous = null;
            while ((current = temp.GetNextNode()) != null)
            {
                if (previous == null)
                {
                    previous = current;
                    continue;
                }
                if (SetPathSegment(previous, current))
                {
                    previous = current;
                }
                else
                {
                    Debug.LogError("DisplayPath failed, passed a null Node!");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Sets all Nodes between Arrow endpoint to a certain nodeState
    /// </summary>
    /// <param name="arrow">Arrow to be set</param>
    /// <param name="state">State that the Node will be set to (navigational or emmpty)</param>
    protected void SetNodeOccupation(Arrow arrow, Node.nodeStates state)
    {
        bool vertical = arrow.Origin.Coordinate.x == arrow.Destination.Coordinate.x;
        if (vertical == (arrow.Origin.Coordinate.y == arrow.Destination.Coordinate.y))
        {
            Debug.LogError("Error marking an arrow occupied! The Arrow could be invalid!");
            return;
        }
        int start = vertical ? arrow.Origin.Coordinate.y : arrow.Origin.Coordinate.x;
        int end = vertical ? arrow.Destination.Coordinate.y : arrow.Destination.Coordinate.x;
        int constCoord = vertical ? arrow.Origin.Coordinate.x : arrow.Origin.Coordinate.y;
        if (start > end)
        {
            int temp = start;
            start = end;
            end = temp;
        }


        for (int i = start; i <= end; i++)
        {
            if (vertical)
            {
                worldGrid.getAt(constCoord, i).Occupied = state;
            }
            else
            {
                worldGrid.getAt(i, constCoord).Occupied = state;
            }
        }
    }

}
