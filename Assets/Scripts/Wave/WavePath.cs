using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper for a Queue of Nodes which allows Agents to follow a Path
/// </summary>
public class WavePath {


    /*-----------public variables-----------*/
    /// <summary>
    /// Where the Path starts (get only)
    /// </summary>
    public Node StartNode
    {
        get
        {
            return startNode;
        }
    }
    /// <summary>
    /// Where the Path ends (get only)
    /// </summary>
    public Node EndNode
    {
        get
        {
            return endNode;
        }
    }


    /*-----------private variables-----------*/
    private Node startNode;
    private Node endNode;
    /// <summary>
    /// Holds the Path as a Queue
    /// </summary>
    private Queue<Node> NodeQueue = new Queue<Node>();


    /*-----------public functions-----------*/
    /// <summary>
    /// Creates the object based on a given Node Queue
    /// </summary>
    /// <param name="queue">Node Queue which should have at least a valid end and start point</param>
    public WavePath(Queue<Node> queue)
    {
        NodeQueue = new Queue<Node>(queue);
        endNode = queue.Peek();
        NodeQueue = new Queue<Node>(NodeQueue);
        startNode = queue.Peek();
    }

    /// <summary>
    /// Allows for copy construction
    /// </summary>
    /// <remarks>
    /// Uses the parametrized constructor using the other Node Queue
    /// </remarks>
    /// <param name="other"></param>
    public WavePath(WavePath other) : this(other.NodeQueue) {}

    /// <summary>
    /// Removes the next Node and returns it
    /// </summary>
    /// <returns>The next Node or null if the Queue is empty</returns>
    public Node GetNextNode()
    {
        if(NodeQueue.Count > 0)
        {
            return NodeQueue.Dequeue();
        } else
        {
            return null;
        }
        
    }

    /// <summary>
    /// Allows for printing of entire Node Queue to be viewed
    /// </summary>
    /// <returns>String to be outputted</returns>
    public override string ToString()
    {
        List<Node> NodeList = new List<Node>(NodeQueue);
        string ret = base.ToString() + " [";
        foreach(Node n in NodeList)
        {
            ret += "(" + n.Coordinate.x + ", " + n.Coordinate.y + "),";
        }
        if (NodeList.Count > 0)
        {
            ret = ret.Remove(ret.Length - 1, 1);
        }
        ret += "]";
        return ret;
    }

}
