using CatchCo;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manages the waves, allows for building of the WavePaths and Waves
/// </summary>
[ExecuteInEditMode]
public class WaveManager : MonoBehaviour
{
    /*-----------public variables-----------*/
    /// <summary>
    /// Mandatory prefab so a Wave can be created and used
    /// </summary>
    public Wave wavePrefab;
    /// <summary>
    /// Manatory prefab so an Arrow can be drawn and placed
    /// </summary>
    public Arrow arrowPrefab;
    /// <summary>
    /// How much the arrow will overlay on the grid
    /// </summary>
    /// <remarks>
    /// Default works fine
    /// </remarks>
    public float arrowOffset = 0.1f;
    /// <summary>
    /// Temperary prefab to be expanded in to a list of possible Agents
    /// </summary>
    public Agent agentPrefab;
    /// <summary>
    /// Where the Agent must spawn and where the first Arrow must start
    /// </summary>
    //public GridArea startArea;
    /// <summary>
    /// Where the Agent will be terminated and where the last Arrow must end
    /// </summary>
    //public GridArea endArea;

    public Level thisLevel;
    public List<WavePath> wavePathList = new List<WavePath>();


    /*-----------private variables-----------*/
    /// <summary>
    /// Reference to the WorldGrid object
    /// </summary>
    /// <remarks>
    /// Necessary for raycasting to Nodes
    /// </remarks>
    private WorldGrid worldGrid;
    /// <summary>
    /// List of Waves to be pushed to the player
    /// </summary>
    /// <remarks>
    /// Not implemented yet
    /// </remarks>
    private List<Wave> waveList = new List<Wave>();
    /// <summary>
    /// The Wave that the Manager is currently handling and editing
    /// </summary>
    private Wave currentWave;
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
    /// The Stack of Arrows that visually make up the path to be converted to a WavePath when pushed
    /// </summary>
    //private Stack<Arrow> arrowStack = new Stack<Arrow>();

    [SerializeField]
    private ArrowContainer arrowContainer;


    /*-----------private MonoBehavior functions-----------*/
    /// <summary>
    /// Assigns the WorldGrid reference and currently creates a test Wave
    /// </summary>
    private void Start()
    {
        if (!EditorApplication.isPlaying)
        {
            thisLevel.LoadLevel();
            return;
        }
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }
        Load();
        /*test area*/
        Wave newWave = Instantiate(wavePrefab, this.transform) as Wave;
        waveList.Add(newWave);
        currentWave = waveList[0];
        /*end of test area*/
    }

    /// <summary>
    /// Call the neccessary functions for operation
    /// </summary>
    private void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }
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
            Save();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Load();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Clear();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            MakeAgentInWave(currentWave);
        }
    }

    private void MakeAgentInWave(Wave wave)
    {
        if (wavePathList.Count > 0)
        {
            int pathIndex = Random.Range(0, wavePathList.Count);
            WavePath currentPath = wavePathList[pathIndex];
            wave.AddNewAgent(agentPrefab, currentPath);
            Debug.Log("Made AgentPath!");
        } else
        {
            Debug.LogError("Wave list is empty!");
        }
    }

    private void Clear()
    {
        Debug.Log("Cleared!");
        wavePathList = new List<WavePath>();
        thisLevel.DeleteLevel();
    }

    private void Save()
    {
        if (wavePathList != null && wavePathList.Count > 0)
        {
            thisLevel.SetLevel(wavePathList);
            thisLevel.SaveLevel();

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

    public void Load()
    {
        List<SerializableWavePath> tempList = thisLevel.LoadLevel();
        if (tempList != null)
        {
            thisLevel.SetLevel(tempList);
            if (worldGrid != null)
            {
                UseLevel(tempList);
                foreach (WavePath path in wavePathList)
                {
                    Debug.Log("Loaded: " + path);
                }
                DisplayPath(wavePathList);
            }
        }
        else
        {
            Debug.LogError("Loading failed!");
        }
    }


    /*-----------private functions-----------*/
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
            arrowContainer.RemoveArrows(NodePointedAt);
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

    private Node GetCardinalNode(Node target, Node origin)
    {
        int distanceY = Mathf.Abs(target.Coordinate.y - origin.Coordinate.y);
        int distanceX = Mathf.Abs(target.Coordinate.x - origin.Coordinate.x);
        
        if(distanceX >= distanceY)
        {
            return worldGrid.getAt(target.Coordinate.x, origin.Coordinate.y);
        } else
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
                if (currEnd == currStart || currEnd.Occupied == true) //end and start cannot be the same
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
                SetPathSegment(currStart, currEnd);
            }

        }
    }

    private void DisplayPath(List<WavePath> wavePaths)
    {
        foreach(WavePath path in wavePaths)
        {
            WavePath temp = new WavePath(path);
            Node current = null;
            Node previous = null;
            while((current = temp.GetNextNode()) != null)
            {
                if(previous == null)
                {
                    previous = current;
                    continue;
                }

                drawArrow = Instantiate(arrowPrefab, transform) as Arrow;
                SetPathSegment(previous, current);
                previous = current;
            }
        }
    }


    /// <summary>
    /// When finished placing an Arrow, the Arrow will be placed and adhere to the Grid
    /// </summary>
    /// <param name="start">The Node in which the base of the Arrow is in</param>
    /// <param name="end">The Node in which the tip of the Arrow is in</param>
    private void SetPathSegment(Node start, Node end)
    {

        if (start == null || end == null)
        {
            Debug.LogError("Passed invalid nodes to create segment!");
            return;
        }
        start.Occupied = true;
        end.Occupied = true;
        drawArrow.PlaceArrow(start, end, arrowOffset);
        arrowContainer.AddArrowToContainer(drawArrow);

        currStart = null;
        currEnd = null;
    }

    /// <summary>
    /// Pushes the WavePath to the currentWave's Path list to be used in that object
    /// </summary>
    /// <returns>If the Path is valid, returns true, else false</returns>
    private bool PushPath()
    {
        wavePathList = arrowContainer.ToWavePaths();
        return true;
    }

    private void UseLevel(List<SerializableWavePath> paths)
    {
        wavePathList = new List<WavePath>();
        foreach(SerializableWavePath path in paths)
        {
            wavePathList.Add(new WavePath(path, worldGrid));
        }
    }

    

}
