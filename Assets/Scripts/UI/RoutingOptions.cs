using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoutingOptions : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{

    public AgentAttribute currentAttribute;
    public RouterBuilding parentTower;
    public bool Over = false;

    private void Start()
    {
        parentTower = transform.parent.parent.gameObject.GetComponent<RouterBuilding>();
        if (parentTower == null)
        {
            Debug.LogError("Cannot find the Router Building object!");
            return;
        }
        if (!parentTower.name.Contains("Test Router"))
        {
            Debug.LogError("Parent object is not named as expected!");
            return;
        }
        PopulateDropdowns();
    }

    private void PopulateDropdowns()
    {
        List<string> colorList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.possibleColors)));
        List<string> speedList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.possibleSpeeds)));
        List<string> sizeList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.possibleSizes)));
        List<List<string>> enumList = new List<List<string>>();
        enumList.Add(colorList);
        enumList.Add(sizeList);
        enumList.Add(speedList);

        if (transform.childCount != enumList.Count)
        {
            Debug.LogError("Number of dropdown menus do not mirror number of attributes! Remove or add more menus!");
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Dropdown currentDropdown = transform.GetChild(i).GetComponent<Dropdown>();
            if (currentDropdown == null)
            {
                Debug.LogError("Cannot find child of RoutingOptions!");
                return;
            }
            currentDropdown.onValueChanged.AddListener(delegate {
                UpdateFilter();
            });
            currentDropdown.ClearOptions();
            currentDropdown.AddOptions(enumList[i]);
            if(enumList[i].Count - 1 != 3)
            {
                Debug.LogError("Enum list length is not as expected!");
            }
            currentDropdown.value = enumList[i].Count - 1;
        }
        UpdateFilter();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Over = false;
    }

    public void UpdateFilter()
    {
        currentAttribute.Color = (AgentAttribute.possibleColors)transform.Find("Color").GetComponent<Dropdown>().value;
        currentAttribute.Size = (AgentAttribute.possibleSizes)transform.Find("Size").GetComponent<Dropdown>().value;
        currentAttribute.Speed = (AgentAttribute.possibleSpeeds)transform.Find("Speed").GetComponent<Dropdown>().value;
        //change to support more than one filter later
        if (parentTower.filter != null)
        {
            if (parentTower.filter.Count > 0)
            {
                parentTower.filter.Clear();
            }
            parentTower.filter.Add(currentAttribute);
        }
    }

}
