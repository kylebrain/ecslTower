﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSelectionScript : MonoBehaviour {

    public LevelPanel levelPanelPrefab;
    public float levelSpacing = 20f;

    private float levelPanelWidth;
    private bool showingHidden = false;

    private void Start()
    {
        if(transform.childCount > 0)
        {
            Debug.LogError("Level Selection should not start with any child objects!");
        }
        DisplayLevels(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            DisplayLevels(showingHidden = !showingHidden);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            LevelUnlocking.ResetUnlocked();
            DisplayLevels(false);
        }
    }

    private void DisplayLevels(bool showHidden)
    {
        foreach(Transform child in transform) //change this if the error was triggered
        {
            if(child.GetComponent<LevelPanel>() != null)
            {
                Destroy(child.gameObject);
            }  
        }
        levelPanelWidth = levelPanelPrefab.GetComponent<RectTransform>().sizeDelta.x;
        List<Map> mapList = Resources.LoadAll<Map>("Maps").ToList();
        mapList = mapList.OrderBy(map => map.levelNumber).ToList();

        if(!showHidden)
        {
            mapList.RemoveAll(m => m.hidden);
        }

        int levelCount = mapList.Count;
        float midPoint = (levelCount - 1) / 2f;
        for (int i = 0; i < levelCount; i++)
        {
            Map currentMap = mapList[i];
            LevelPanel currentPanel = Instantiate(levelPanelPrefab, transform);
            currentPanel.Init(currentMap.levelNumber, currentMap.name, showHidden || currentMap.GetUnlocked()); //if we show hidden all are on the table
            currentPanel.transform.localPosition = new Vector3((i - midPoint) * (levelPanelWidth + levelSpacing), 0f);
        }
    }

}