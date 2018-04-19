using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePath {
    private Node startNode;
    private Node endNode;
    private List<Node> PathList = new List<Node>();
    private int nodeIndex;

    /// <summary>
    /// Creates a basic path given a starting Node and a final Node and sets these two values;
    /// </summary>
    /// <param name="start">Where the path begins</param>
    /// <param name="end">Where the path terminates</param>
    public void InitializePath(Node start, Node end)
    {
        PathList.Clear();
        startNode = start;
        endNode = end;
        PathList.Add(startNode);
        PathList.Add(endNode);
        nodeIndex = -1;
    }

    /// <summary>
    /// Adds a midpoint Node before the final Node but after all other Nodes
    /// </summary>
    /// <param name="midpoint">The Node to be inserted</param>
    /// <returns>The midpoint Node that was passed</returns>
    public Node AddMidpoint(Node midpoint)
    {
        PathList.Insert(PathList.Count - 2, midpoint);
        return midpoint;
    }

    /// <summary>
    /// Will retrieve the next Node in the list and increment the current Node in
    /// </summary>
    /// <returns>Return the next Node but returns null if currently on the last Node or the startNode if called for the first time</returns>
    public Node GetNextNode()
    { 
        if(nodeIndex == PathList.Count - 1)
        {
            return null;
        }

        nodeIndex++;
        return PathList[nodeIndex];
    }

}
