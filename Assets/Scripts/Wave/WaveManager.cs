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

    //test

    private void Start()
    {
        Wave newWave = Instantiate(wavePrefab, this.transform) as Wave;
        waveList.Add(newWave);
        currentWave = waveList[0];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currStart != null)
            {
                currEnd = worldGrid.getRaycastNode();
                if(currEnd == currStart)
                {
                    currEnd = null;
                }
                if (currEnd != null)
                {
                    Debug.Log("End Point set to: " + currEnd.name);
                }
            }
            else
            {
                currStart = worldGrid.getRaycastNode();
                if (currStart != null)
                {
                    Debug.Log("Start Point set to: " + currStart.name);
                }
            }

            if (currStart != null && currEnd != null)
            {
                WavePath newPath = new WavePath();
                newPath.InitializePath(currStart, currEnd);
                currentWave.pathList.Add(newPath);
            }
        }
    }


}
