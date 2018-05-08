using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public WorldGrid worldGrid;
    List<Wave> waveList = new List<Wave>();
    private Wave currentWave;
    private Node currStart = null;
    private Node currEnd = null;
    public Wave wavePrefab;
    public Arrow arrowPrefab;
    private Arrow drawArrow = null;
    public float arrowOffset = 0.5f;
    private WavePath newPath = new WavePath();
    Stack<Arrow> arrowStack = new Stack<Arrow>();
    //Stack<Node> nodeStack = new Stack<Node>();
    //private Node lastNodeOfNodeQueue = null;
    public GridArea startArea;
    public GridArea endArea;

    //test

    private void Start()
    {
        Wave newWave = Instantiate(wavePrefab, this.transform) as Wave;
        waveList.Add(newWave);
        currentWave = waveList[0];
    }

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

    private void RemoveArrow(Arrow currentArrow)
    {
        currentArrow.Destination.Occupied = Node.nodeStates.empty;
        if (arrowStack.Count == 1)
        {
            currentArrow.Origin.Occupied = Node.nodeStates.empty;
        }
        arrowStack.Pop();
        Destroy(currentArrow.gameObject);
    }

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
                if (currEnd == currStart || currEnd.Occupied != Node.nodeStates.empty) //end and start cannot be the same
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

    private void SetPathSegment(Node start, Node end)
    {

        if(start == null || end == null)
        {
            Debug.LogError("Passed invalid nodes to create segment!");
            return;
        }
        start.Occupied = Node.nodeStates.navigation;
        end.Occupied = Node.nodeStates.navigation;
        drawArrow.PlaceArrow(start, end, arrowOffset);
        Arrow newArrow = drawArrow;
        arrowStack.Push(newArrow);

        currStart = null;
        currEnd = null;
    }

    private bool PushPath()
    {
        if (!endArea.Contains(arrowStack.Peek().Destination.Coordinate))
        {
            Debug.LogError("End arrow does not end in end area!");
            return false;
        }

        Stack<Arrow> tempArrowStack = new Stack<Arrow>(arrowStack); //copy constructor of stack reverses order
        List<Node> nodeList = new List<Node>();

        Arrow startArrow = tempArrowStack.Pop();

        if (!startArea.Contains(startArrow.Origin.Coordinate))
        {
            Debug.LogError("Start arrow does not begin in start area!");
            return false;
        }

        nodeList.Add(startArrow.Origin);
        nodeList.Add(startArrow.Destination);

        while (tempArrowStack.Count > 0)
        {
            Arrow currentArrow = tempArrowStack.Pop();
            nodeList.Add(currentArrow.Destination);
        }


        newPath.InitializePath(nodeList);
        currentWave.pathList.Add(newPath);
        Debug.Log(newPath);
        return true;
    }
}
