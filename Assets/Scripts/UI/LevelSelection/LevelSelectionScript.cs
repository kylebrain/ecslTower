using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSelectionScript : MonoBehaviour {

    public LevelPanel levelPanelPrefab;
    public float levelSpacing = 20f;

    private float levelPanelWidth;

    private void Awake()
    {
        levelPanelWidth = levelPanelPrefab.GetComponent<RectTransform>().sizeDelta.x;
        Map[] mapArray = Resources.LoadAll<Map>("Levels");
        mapArray = mapArray.OrderBy(map => map.levelNumber).ToArray();
        int levelCount = mapArray.Length;
        float midPoint = (levelCount - 1) / 2f;
        for(int i = 0; i < levelCount; i++)
        {
            Map currentMap = mapArray[i];
            LevelPanel currentPanel = Instantiate(levelPanelPrefab, transform);
            currentPanel.LevelName = currentMap.name;
            currentPanel.transform.localPosition = new Vector3((i - midPoint) * (levelPanelWidth + levelSpacing), 0f);
        }
    }

}
