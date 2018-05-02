using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the waves, allows for building of the WavePaths and Waves
/// </summary>
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
    public GridArea startArea;
    /// <summary>
    /// Where the Agent will be terminated and where the last Arrow must end
    /// </summary>
    public GridArea endArea;


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
    private Stack<Arrow> arrowStack = new Stack<Arrow>();


    /*-----------private MonoBehavior functions-----------*/
    /// <summary>
    /// Assigns the WorldGrid reference and currently creates a test Wave
    /// </summary>
    private void Start()
    {
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }

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

            List<Arrow> tempArrowList = new List<Arrow>(arrowStack);

            if (tempArrowList.FindIndex(a => a.Origin == NodePointedAt) >= 0) //if it is actually a selected node
            {
                Arrow currentArrow;
                while(arrowStack.Count > 0 && (currentArrow = arrowStack.Peek()).Destination != NodePointedAt)
                {
                    RemoveArrow(currentArrow);
                }
            } else if (tempArrowList.FindIndex(a => a.Destination == NodePointedAt) >= 0)
            {
                Arrow endArrow = arrowStack.Peek();
                if(endArrow.Destination == NodePointedAt)
                {
                    RemoveArrow(endArrow);
                }
            }
        }
    }

    /// <summary>
    /// Will remove the passed Arrow from the Stack and Destroy the GameObject
    /// </summary>
    /// <param name="currentArrow">The Arrow to de removed, must be in Stack</param>
    private void RemoveArrow(Arrow currentArrow)
    {
        currentArrow.Destination.Occupied = false;
        if (arrowStack.Count == 1)
        {
            currentArrow.Origin.Occupied = false;
        }
        arrowStack.Pop();
        Destroy(currentArrow.gameObject);
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
                drawArrow.DrawArrow(currStart.transform.position, nodePointedAt.transform.position, arrowOffset);
            }
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
                if (arrowStack.Count == 0 ? (startArea.Contains(currentNode.Coordinate)) : (arrowStack.Peek().Destination == currentNode))
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

            else //if start node has been select, select the end node
            {
                currEnd = currentNode;
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

    /// <summary>
    /// When finished placing an Arrow, the Arrow will be placed and adhere to the Grid
    /// </summary>
    /// <param name="start">The Node in which the base of the Arrow is in</param>
    /// <param name="end">The Node in which the tip of the Arrow is in</param>
    private void SetPathSegment(Node start, Node end)
    {

        if(start == null || end == null)
        {
            Debug.LogError("Passed invalid nodes to create segment!");
            return;
        }
        start.Occupied = true;
        end.Occupied = true;
        drawArrow.PlaceArrow(start, end, arrowOffset);
        Arrow newArrow = drawArrow;
        arrowStack.Push(newArrow);

        currStart = null;
        currEnd = null;
    }

    /// <summary>
    /// Pushes the WavePath to the currentWave's Path list to be used in that object
    /// </summary>
    /// <returns>If the Path is valid, returns true, else false</returns>
    private bool PushPath()
    {
        if(arrowStack.Count == 0)
        {
            Debug.LogError("No arrows have been created!");
            return false;
        }

        if (!endArea.Contains(arrowStack.Peek().Destination.Coordinate))
        {
            Debug.LogError("End arrow does not end in end area!");
            return false;
        }

        Stack<Arrow> tempArrowStack = new Stack<Arrow>(arrowStack); //copy constructor of stack reverses order
        Queue<Node> nodeQueue = new Queue<Node>();

        Arrow startArrow = tempArrowStack.Pop();

        if (!startArea.Contains(startArrow.Origin.Coordinate))
        {
            Debug.LogError("Start arrow does not begin in start area!");
            return false;
        }

        nodeQueue.Enqueue(startArrow.Origin);
        nodeQueue.Enqueue(startArrow.Destination);

        while (tempArrowStack.Count > 0)
        {
            Arrow currentArrow = tempArrowStack.Pop();
            nodeQueue.Enqueue(currentArrow.Destination);
        }

        WavePath newPath = new WavePath(nodeQueue);
        currentWave.AddNewAgent(agentPrefab, newPath);
        Debug.Log(newPath);
        return true;
    }
}
