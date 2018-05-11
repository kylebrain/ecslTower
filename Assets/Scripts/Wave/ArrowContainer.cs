using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrowContainer
{

    public List<Stack<Arrow>> arrowStacks = new List<Stack<Arrow>>();
    public List<GridArea> startAreas = new List<GridArea>();
    public List<GridArea> endAreas = new List<GridArea>();

    public void RemoveArrows(Node selectedNode)
    {
        foreach (Stack<Arrow> arrowStack in arrowStacks)
        {
            List<Arrow> tempArrowList = new List<Arrow>(arrowStack);
            if (tempArrowList.FindIndex(a => a.Origin == selectedNode) >= 0) //if it is actually a selected node
            {
                Arrow currentArrow;
                while (arrowStack.Count > 0 && (currentArrow = arrowStack.Peek()).Destination != selectedNode)
                {
                    RemoveArrow(currentArrow, arrowStack);
                }
            }
            else if (tempArrowList.FindIndex(a => a.Destination == selectedNode) >= 0)
            {
                Arrow endArrow = arrowStack.Peek();
                if (endArrow.Destination == selectedNode)
                {
                    RemoveArrow(endArrow, arrowStack);
                }
            }
        }
    }

    private void RemoveArrow(Arrow currentArrow, Stack<Arrow> arrowStack) //might need to be passed by ref
    {
        currentArrow.Destination.Occupied = Node.nodeStates.empty;
        if (arrowStack.Count == 1)
        {
            currentArrow.Origin.Occupied = Node.nodeStates.empty;
        }
        arrowStack.Pop();
        currentArrow.KillArrrow();
    }

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

        return false;
    }

    public Arrow AddArrowToContainer(Arrow arrow)
    {
        foreach (Stack<Arrow> arrowStack in arrowStacks)
        {
            if (arrowStack.Count > 0 && arrowStack.Peek().Destination == arrow.Origin)
            {
                arrowStack.Push(arrow);
                return arrow;
            }
        }
        foreach (GridArea startArea in startAreas)
        {
            if (startArea.Contains(arrow.Origin.Coordinate))
            {
                Stack<Arrow> newStack = new Stack<Arrow>();
                arrowStacks.Add(newStack);
                newStack.Push(arrow);
                return arrow;
            }
        }

        return null;
    }

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
                continue;
            }

            Stack<Arrow> tempArrowStack = new Stack<Arrow>(arrowStack); //copy constructor of stack reverses order
            Queue<Node> nodeQueue = new Queue<Node>();

            Arrow startArrow = tempArrowStack.Pop();

            Valid = false;
            foreach (GridArea startArea in startAreas)
            {
                if (startArea.Contains(startArrow.Origin.Coordinate))
                {
                    //Debug.LogError("Start arrow does not begin in start area!");
                    Valid = true;
                    break;
                }
            }
            if (!Valid)
            {
                continue;
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
