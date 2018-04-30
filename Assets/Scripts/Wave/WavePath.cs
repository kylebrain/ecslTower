using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePath {
    public Node StartNode
    {
        get
        {
            return startNode;
        }
    }
    public Node EndNode
    {
        get
        {
            return endNode;
        }
    }
    private Node startNode;
    private Node endNode;
    private Queue<Node> NodeQueue = new Queue<Node>();

    public WavePath(Queue<Node> queue)
    {
        NodeQueue = new Queue<Node>(queue);
        endNode = queue.Peek();
        NodeQueue = new Queue<Node>(NodeQueue);
        startNode = queue.Peek();
    }

    public WavePath(WavePath other) : this(other.NodeQueue) {}

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

    public override string ToString()
    {
        List<Node> NodeList = new List<Node>(NodeQueue);
        string ret = base.ToString() + " [";
        foreach(Node n in NodeList)
        {
            ret += "(" + ((int)n.transform.position.x) + ", " + ((int)n.transform.position.z) + "),";
        }
        if (NodeList.Count > 0)
        {
            ret = ret.Remove(ret.Length - 1, 1);
        }
        ret += "]";
        return ret;
    }

}
