using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Wrapper for a Queue of Nodes which allows Agents to follow a Path
/// </summary>
[System.Serializable]
public class WavePath {

    #region Public Variables

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

    /// <summary>
    /// The Queue of Nodes contained in the WavePath (get only)
    /// </summary>
    public Queue<Node> NodeQueue
    {
        get
        {
            return nodeQueue;
        }
    }

    /// <summary>
    /// Returns a List of Nodes based on the Node Queue
    /// </summary>
    public List<Node> NodeList
    {
        get
        {
            return new List<Node>(nodeQueue);
        }
    }

    #endregion

    #region Private Variables

    /// <summary>
    /// Local StartNode
    /// </summary>
    private Node startNode;

    /// <summary>
    /// Local EndNode
    /// </summary>
    private Node endNode;

    /// <summary>
    /// Holds the Path as a Queue
    /// </summary>
    private Queue<Node> nodeQueue = new Queue<Node>();

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the object based on a given Node Queue
    /// </summary>
    /// <param name="queue">Node Queue which should have at least a valid end and start point</param>
    public WavePath(Queue<Node> queue)
    {
        nodeQueue = new Queue<Node>(queue);
        endNode = queue.Peek();
        nodeQueue = new Queue<Node>(nodeQueue);
        startNode = queue.Peek();
    }

    /// <summary>
    /// Generates a WavePath objects based on a SerializableWavePath
    /// </summary>
    /// <param name="path">The SerializedWavePath to convert</param>
    /// <param name="grid">The WorldGrid which will contain the WavePath</param>
    public WavePath(SerializableWavePath path, WorldGrid grid)
    {
        List<Node> tempNodeList = new List<Node>();
        foreach (Coordinate c in path.list)
        {
            tempNodeList.Add(grid.getAt(c.x, c.y));
        }
        nodeQueue = new Queue<Node>(tempNodeList);
        startNode = nodeQueue.Peek();
        endNode = tempNodeList[tempNodeList.Count - 1];
    }

    /// <summary>
    /// Allows for copy construction
    /// </summary>
    /// <remarks>
    /// Uses the parametrized constructor using the other Node Queue
    /// </remarks>
    /// <param name="other"></param>
    public WavePath(WavePath other) : this(other.nodeQueue) {}

    public WavePath(Vector2[] other)
    {
        List<Node> tempNodeList = new List<Node>();
        foreach (Vector2 vector in other)
        {
            tempNodeList.Add(Node.getAt(vector));
        }
        nodeQueue = new Queue<Node>(tempNodeList);
        startNode = nodeQueue.Peek();
        endNode = tempNodeList[tempNodeList.Count - 1];
    }

    #endregion



    /// <summary>
    /// Removes the next Node and returns it
    /// </summary>
    /// <returns>The next Node or null if the Queue is empty</returns>
    public Node GetNextNode()
    {
        if(nodeQueue.Count > 0)
        {
            return nodeQueue.Dequeue();
        } else
        {
            return null;
        }
        
    }

    public Vector2[] ToVector2Array()
    {
        return nodeQueue.ToArray().Select(x => new Vector2(x.Coordinate.x, x.Coordinate.y)).ToArray();
    }

    #region Object Overrides

    /// <summary>
    /// Allows for printing of entire Node Queue to be viewed
    /// </summary>
    /// <returns>String to be outputted</returns>
    public override string ToString()
    {
        if(nodeQueue == null)
        {
            return "null";
        }
        List<Node> NodeList = new List<Node>(nodeQueue);
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

    //***might cause problems if not changed to mirror the Equals function
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Check equality based on the order of Nodes
    /// </summary>
    /// <param name="obj">Other WavePath to check</param>
    /// <returns>True if equal, false if not</returns>
    public override bool Equals(object obj)
    {
        if(obj.GetType() != GetType())
        {
            return false;
        }
        List<Node> otherNodeList = new List<Node>(((WavePath)obj).nodeQueue);
        List<Node> thisNodeList = new List<Node>(nodeQueue);
        if(thisNodeList.Count != otherNodeList.Count)
        {
            return false;
        }
        for(int i = 0; i < thisNodeList.Count; i++)
        {
            if(thisNodeList[i] != otherNodeList[i])
            {
                return false;
            }
        }
        return true;
    }

    #endregion

}
