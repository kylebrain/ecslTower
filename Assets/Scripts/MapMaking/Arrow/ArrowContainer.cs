using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the visual representation of the paths
/// </summary>
[System.Serializable]
public class ArrowContainer
{
    /// <summary>
    /// Contains paths represented by Stacks of Arrows
    /// </summary>
    public List<Stack<Arrow>> arrowStacks = new List<Stack<Arrow>>();
    /// <summary>
    /// The GridAreas that a path can start in, set in inspector
    /// </summary>
    public List<GridArea> startAreas = new List<GridArea>();
    /// <summary>
    /// The GridAreas that a path can end in, set in inspector
    /// </summary>
    public List<GridArea> endAreas = new List<GridArea>();

    public List<List<Arrow>> ArrowLists
    {
        get
        {
            List<List<Arrow>> list = new List<List<Arrow>>();
            foreach(Stack<Arrow> stack in arrowStacks)
            {
                list.Add(new List<Arrow>(stack));
            }
            return list;
        }
    }

    /// <summary>
    /// Will remove the desired Arrows from the arrowStack
    /// </summary>
    /// <param name="selectedNode">All Arrows after this Node will be removed</param>
    /// <returns>List of Arrows to be Destroyed, handled outside of the class</returns>
    public List<Arrow> RemoveArrows(Node selectedNode)
    {
        List<Arrow> ret = new List<Arrow>();
        foreach (Stack<Arrow> arrowStack in arrowStacks)
        {
            List<Arrow> tempArrowList = new List<Arrow>(arrowStack);
            if (tempArrowList.FindIndex(a => a.Origin == selectedNode) >= 0) //if it is actually a selected node
            {
                Arrow currentArrow;
                while (arrowStack.Count > 0 && (currentArrow = arrowStack.Peek()).Destination != selectedNode) //removes every after it
                {
                    ret.Add(currentArrow);
                    RemoveArrow(currentArrow, arrowStack);
                }
            }
            else if (tempArrowList.FindIndex(a => a.Destination == selectedNode) >= 0) //will remove the first arrow in a stack if its destination is clicked
            {
                Arrow endArrow = arrowStack.Peek();
                if (endArrow.Destination == selectedNode)
                {
                    ret.Add(endArrow);
                    RemoveArrow(endArrow, arrowStack);
                }
            }
        }

        //removing segments

        //checks every arrow on main path to see if they contain a segment
        //segmentList is a list of all arrows that are segmented off the main path
        List<Arrow> segmentList = RemoveSegments(ret);
        ret.AddRange(segmentList);
        
        while(segmentList.Count > 0) //repeat until no segments are found
        {
            //then check every segmented arrow to see if they have segments
            segmentList = RemoveSegments(segmentList);
            ret.AddRange(segmentList);
        }

        return ret;
    }

    private List<Arrow> RemoveSegments(List<Arrow> checkArrows)
    {
        List<Arrow> segmentRet = new List<Arrow>();
        foreach (Arrow checkArrow in checkArrows) //checks each arrow of the passed list
        {
            foreach (Stack<Arrow> segmentCheckStack in arrowStacks) //checks every stack
            {
                if (segmentCheckStack.Contains(checkArrow) || segmentCheckStack.Count <= 0)
                {
                    continue;
                }

                /*
                List<Arrow> segmentCheckList = new List<Arrow>(segmentCheckStack);
                Node target = segmentCheckList[segmentCheckList.Count - 1].Origin; //is the origin of the root arrow
                */

                Node target = GetOriginNodeFromStack(segmentCheckStack); //change to above if function doesn't work

                if (target != null && target.IsBetween(checkArrow)) //between the arrow we are checking?
                {
                    while (segmentCheckStack.Count > 0)
                    {
                        Arrow currentSegmentStackArrow = segmentCheckStack.Peek();
                        segmentRet.Add(currentSegmentStackArrow);
                        RemoveArrow(currentSegmentStackArrow, segmentCheckStack);
                    }
                }
            }
        }
        return segmentRet;
    }

    /// <summary>
    /// Removes an Arrow from the Stack and sets its endpoints to empty
    /// </summary>
    /// <param name="currentArrow">The Arrow to be removed</param>
    /// <param name="arrowStack">The Stack that has the certain Arrow as its front</param>
    private void RemoveArrow(Arrow currentArrow, Stack<Arrow> arrowStack) //might need to be passed by ref
    {
        if (currentArrow != arrowStack.Peek())
        {
            Debug.LogError("Invalid Arrow and Stack, Arrow must be on top of the Stack!");
            return;
        }
        currentArrow.Destination.Occupied = Node.nodeStates.empty;
        if (arrowStack.Count == 1)
        {
            currentArrow.Origin.Occupied = Node.nodeStates.empty;
        }
        arrowStack.Pop();
    }

    /// <summary>
    /// Checks to see if an Arrow can originate from a Node
    /// </summary>
    /// <param name="selectedNode">The Node to be checked</param>
    /// <returns>If the Node is the end of an existing Arrow Stack or is in a StartArea</returns>
    public bool IsVaildNode(Node selectedNode)
    {
        foreach (Stack<Arrow> arrowStack in arrowStacks)
        {
            if (arrowStack.Count > 0 && arrowStack.Peek().Destination == selectedNode)
            {
                return true;
            }
        }

        foreach (GridArea startArea in startAreas)
        {
            if (startArea.Contains(selectedNode.Coordinate))
            {
                return true;
            }
        }

        if (FindIntersectingArrow(selectedNode) != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds an Arrow to the Stack based on where it should be added, creates a new Stack if originating in a StartArea
    /// </summary>
    /// <param name="arrow">Arrow to be added, should be checked the IsValidNode function</param>
    /// <returns>If succeeds returns the passed Arrow, on fail returns null</returns>
    public Arrow AddArrowToContainer(Arrow arrow)
    {
        foreach (Stack<Arrow> arrowStack in arrowStacks) //if it is at the end of an arrow
        {
            Stack<Arrow> arrowStackFlipped = new Stack<Arrow>(arrowStack);
            foreach(Arrow currentArrow in arrowStackFlipped)
            {
                if(arrow.IsBetween(currentArrow))
                {
                    //Debug.Log(arrow + " is between: " + currentArrow);
                    return null;
                }
            }


            if (arrowStack.Count > 0 && arrowStack.Peek().Destination == arrow.Origin)
            {
                arrowStack.Push(arrow);
                return arrow;
            }
        }
        foreach (GridArea startArea in startAreas) //if it originates in the start area
        {
            if (startArea.Contains(arrow.Origin.Coordinate))
            {
                Stack<Arrow> newStack = new Stack<Arrow>();
                arrowStacks.Add(newStack);
                newStack.Push(arrow);
                return arrow;
            }
        }
        if (FindIntersectingArrow(arrow.Origin) != null) //if it is a branch of another arrow
        {
            Stack<Arrow> newStack = new Stack<Arrow>();
            arrowStacks.Add(newStack);
            newStack.Push(arrow);
            //Debug.Log("Added as a segment!");
            return arrow;
        }

        return null;
    }

    public Arrow FindIntersectingArrow(Node target)
    {
        foreach (Stack<Arrow> arrowStack in arrowStacks)
        {
            List<Arrow> tempList = new List<Arrow>(arrowStack);
            tempList.Reverse();
            foreach (Arrow arrow in tempList)
            {
                if (target.IsBetween(arrow))
                {
                    return arrow;
                }
            }
        }
        //Debug.Log("Could not find an intersecting arrow!");
        return null;
    }

    /// <summary>
    /// Converts the List of Stacks of Arrows to a List of WavePaths
    /// </summary>
    /// <returns>List of WavePaths to be used by the Agents and MapMaker</returns>
    public List<WavePath> ToWavePaths()
    {
        int failedCount = 0;
        List<WavePath> wavePaths = new List<WavePath>();
        List<Stack<Arrow>> segmentedStacks = new List<Stack<Arrow>>();
        List<Queue<Node>> baseQueues = new List<Queue<Node>>(); 

        foreach (Stack<Arrow> arrowStack in arrowStacks)
        {
            if (arrowStack.Count == 0)
            {
                //Debug.LogError("No arrows have been created!");
                continue;
            }

            //checking each endArea to see if it ends there
            bool Valid = false;
            foreach (GridArea endArea in endAreas)
            {
                if (endArea.Contains(arrowStack.Peek().Destination.Coordinate))
                {
                    Valid = true;
                    break;
                }
            }
            if (!Valid)
            {
                failedCount++;
                continue; //this Stack does not end in an endArea
            }

            Stack<Arrow> tempArrowStack = new Stack<Arrow>(arrowStack); //copy constructor of stack reverses order
            Queue<Node> nodeQueue = new Queue<Node>();

            Arrow startArrow = tempArrowStack.Pop();

            //checking each startArea to see if it begins there
            Valid = false;
            foreach (GridArea startArea in startAreas)
            {
                if (startArea.Contains(startArrow.Origin.Coordinate))
                {
                    Valid = true;
                    break;
                }
            }
            if (!Valid)
            {
                segmentedStacks.Add(arrowStack);
                continue; //this start does not end in an endArea
                //does it start in an intermediate Node?
            }

            nodeQueue.Enqueue(startArrow.Origin);
            nodeQueue.Enqueue(startArrow.Destination);

            while (tempArrowStack.Count > 0)
            {
                Arrow currentArrow = tempArrowStack.Pop();
                nodeQueue.Enqueue(currentArrow.Destination);
            }

            baseQueues.Add(nodeQueue);
        }

        //for every segment for every baseQueue
        List<Queue<Node>> demilitarizedList = new List<Queue<Node>>();

        for(int i = 0; i < segmentedStacks.Count; i++)
        {
            Stack<Arrow> currentArrowStack = segmentedStacks[i];
            foreach (Queue<Node> checkNodeQueue in baseQueues)
            {
                Node previous;
                if ((previous = GetIntersectionNode(checkNodeQueue, GetOriginNodeFromStack(currentArrowStack))) != null)
                {
                    Queue<Node> newBaseQueue = GetNodeQueueFromSegmentAndBaseQueue(checkNodeQueue, currentArrowStack, previous);
                    demilitarizedList.Add(newBaseQueue);
                    segmentedStacks.Remove(currentArrowStack);
                    i--;
                }
            }
        }

        //for every segment for every demiliterized queue
        List<Queue<Node>> bufferList = new List<Queue<Node>>(); //buffer so demiliterized isn't changed during the loop
        while (segmentedStacks.Count > 0)
        {
            for (int i = 0; i < segmentedStacks.Count; i++)
            {
                Stack<Arrow> currentArrowStack = segmentedStacks[i];
                foreach (Queue<Node> checkNodeQueue in demilitarizedList)
                {
                    Node previous;
                    if ((previous = GetIntersectionNode(checkNodeQueue, GetOriginNodeFromStack(currentArrowStack))) != null)
                    {
                        Queue<Node> newBaseQueue = GetNodeQueueFromSegmentAndBaseQueue(checkNodeQueue, currentArrowStack, previous);
                        bufferList.Add(newBaseQueue);
                        segmentedStacks.Remove(currentArrowStack);
                        i--;
                    }
                }
            }

            baseQueues.AddRange(demilitarizedList);
            demilitarizedList = new List<Queue<Node>>(bufferList);

        }

        if(demilitarizedList.Count > 0)
        {
            baseQueues.AddRange(demilitarizedList);
        }

        foreach(Queue<Node> currentNodeQueue in baseQueues)
        {
            WavePath newPath = new WavePath(currentNodeQueue);
            wavePaths.Add(newPath);
        }

        if(failedCount > 0)
        {
            Debug.LogWarning("You had " + failedCount + " path" + (failedCount == 1 ? "" : "s") +" fail!\nMake sure every path ends in a sink!");
        }


        if (wavePaths.Count > 0)
        {
            return wavePaths;
        }
        else
        {
            return null;
        }
    }

    private Queue<Node> GetNodeQueueFromSegmentAndBaseQueue(Queue<Node> baseQueue, Stack<Arrow> arrowStack, Node transitionNode)
    {
        if (baseQueue.Count <= 0 || arrowStack.Count <= 0 || transitionNode == null)
        {
            Debug.LogError("Please pass valid arguments");
            return null;
        }
        List<Node> tempList = new List<Node>();
        Queue<Node> baseQueueCopy = new Queue<Node>(baseQueue);
        Node currentNode;
        do
        {
            currentNode = baseQueueCopy.Dequeue();
            tempList.Add(currentNode);
        } while (currentNode != transitionNode);

        Stack<Arrow> arrowStackFlipped = new Stack<Arrow>(arrowStack); //stack copy flips
        Arrow currentArrow = null;
        while(arrowStackFlipped.Count > 0)
        {
            currentArrow = arrowStackFlipped.Pop();
            tempList.Add(currentArrow.Origin);
        }
        tempList.Add(currentArrow.Destination);
        return new Queue<Node>(tempList);
    }

    private Node GetOriginNodeFromStack(Stack<Arrow> passedArrowStack)
    {
        List<Arrow> listArrow = new List<Arrow>(passedArrowStack);
        Node ret = listArrow[listArrow.Count - 1].Origin;
        return ret;
    }

    private Node GetIntersectionNode(Queue<Node> nodeQueue, Node segmentOrigin)
    {
        if(nodeQueue.Count <= 0)
        {
            Debug.LogError("Pass a valid Node Queue!");
            return null;
        }
        Queue<Node> nodeQueueCopy = new Queue<Node>(nodeQueue);
        Node origin = nodeQueueCopy.Dequeue();
        Node destination;
        while(nodeQueueCopy.Count > 0)
        {
            destination = nodeQueueCopy.Dequeue();
            if(segmentOrigin.IsBetween(origin, destination))
            {
                return origin;
            }
            origin = destination;
        }
        return null;
    }

}
