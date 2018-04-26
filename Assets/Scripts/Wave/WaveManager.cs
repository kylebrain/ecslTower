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
    List<Arrow> arrowList = new List<Arrow>();

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
            } else
            {
                Debug.Log("Invalid path!");
            }
        }
    }

    private void RemoveArrowOnClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            List<Arrow> deleteList = new List<Arrow>();
            Node NodePointedAt = worldGrid.getRaycastNode();
            foreach(Arrow a in arrowList)
            {
                Debug.Log("Start: " + a.Origin + ", end: " + a.Destination);
                if(a.Origin == NodePointedAt || a.Destination == NodePointedAt)
                {
                    deleteList.Add(a);
                }
            }
            foreach(Arrow a in deleteList)
            {
                arrowList.Remove(a);
                Destroy(a.gameObject);
            }
        }
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

            if (currStart != null) //if start node has been select, select the end node
            {
                currEnd = currentNode;
                if (currEnd == currStart) //end and start cannot be the same
                {
                    currEnd = null;
                }
                else
                {
                    Debug.Log("End Point set to: " + currEnd.name);
                }
            }
            else //start has not been select so set it
            {
                currStart = currentNode;
                Debug.Log("Start Point set to: " + currStart.name);
                drawArrow = Instantiate(arrowPrefab, transform) as Arrow;
                drawArrow.DrawArrow(currStart.transform.position, worldGrid.getRaycastNode().transform.position, arrowOffset);
            }

            if (currStart != null && currEnd != null) //if both the start and end exist, set the path segment
            {
                SetPathSegment(currStart, currEnd);
            }
        }
    }

    private void SetPathSegment(Node start, Node end)
    {
        drawArrow.PlaceArrow(start, end, arrowOffset);
        Arrow newArrow = drawArrow;
        arrowList.Add(newArrow);
        currStart = null;
        currEnd = null;
    }

    private Arrow ArrowClosestToOrigin(List<Arrow> list)
    {
        Arrow min = null;
        foreach(Arrow a in list)
        {
            if(min == null)
            {
                min = a;
            }
            float currentMagnitude = a.Origin.transform.position.sqrMagnitude;
            float minMagnitude = min.Origin.transform.position.sqrMagnitude;
            if (currentMagnitude < minMagnitude)
            {
                min = a;
            }
        }
        return min;
    }

    private Arrow GetOriginArrow(List<Arrow> list)
    {
        foreach (Arrow originArrow in list)
        {
            int i;
            for (i = 0; i < list.Count; i++)
            {
                Arrow destinationArrow = list[i];
                if(originArrow.Origin == destinationArrow.Destination)
                {
                    break;
                }
            }
            if(i == list.Count)
            {
                return originArrow;
            }
        }
        return null;
    }

    private bool PushPath()
    {
        Arrow start = GetOriginArrow(arrowList);
        
        if(start == null)
        {
            return false;
        }

        Debug.Log("Start node is: " + start.Origin);

        int countArrow = 1;
        List<Node> pathNodes = new List<Node>();
        pathNodes.Add(start.Origin);
        pathNodes.Add(start.Destination);
        Node currentNode = start.Destination;
        while (countArrow < arrowList.Count)
        {
            bool sucess = false;
            foreach(Arrow a in arrowList)
            {
                if(a.Origin == currentNode && a.Destination != currentNode)
                {
                    pathNodes.Add(a.Destination);
                    currentNode = a.Destination;
                    countArrow++;
                    sucess = true;
                    break;
                }
            }
            if (!sucess)
            {
                break;
            }
        }
        if(countArrow < arrowList.Count)
        {
            return false;
        }
        newPath.InitializePath(pathNodes);
        currentWave.pathList.Add(newPath);
        Debug.Log(newPath);
        return true;
    }
}
