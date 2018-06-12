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

                List<Arrow> segmentCheckList = new List<Arrow>(segmentCheckStack);
                Node target = segmentCheckList[segmentCheckList.Count - 1].Origin; //is the origin of the root arrow

                if (target != null && IsBetween(checkArrow, target)) //between the arrow we are checking?
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
            Debug.Log("Added as a segment!");
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
                if (IsBetween(arrow.Origin, arrow.Destination, target))
                {
                    return arrow;
                }
            }
        }
        //Debug.Log("Could not find an intersecting arrow!");
        return null;
    }

    private bool IsBetween(Arrow arrow, Node target)
    {
        return IsBetween(arrow.Origin, arrow.Destination, target);
    }

    private bool IsBetween(Node origin, Node destination, Node target)
    {
        float distBetween = (destination.Coordinate - origin.Coordinate).magnitude;
        float distSegmented = (destination.Coordinate - target.Coordinate).magnitude + (target.Coordinate - origin.Coordinate).magnitude;
        return distBetween == distSegmented;
    }

    /// <summary>
    /// Converts the List of Stacks of Arrows to a List of WavePaths
    /// </summary>
    /// <returns>List of WavePaths to be used by the Agents and MapMaker</returns>
    public List<WavePath> ToWavePaths()
    {
        List<WavePath> wavePaths = new List<WavePath>();
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
                    //Debug.LogError("End arrow does not end in end area!");
                    Valid = true;
                    break;
                }
            }
            if (!Valid)
            {
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

            WavePath newPath = new WavePath(nodeQueue);
            wavePaths.Add(newPath);
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
}
