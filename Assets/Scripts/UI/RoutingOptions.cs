using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the Dropdown menus of the Router to select what to filter
/// </summary>
public class RoutingOptions : MonoBehaviour // , IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The AgentAttribute that the Dropdowns currently represent
    /// </summary>
    public AgentAttribute currentAttribute;
    /// <summary>
    /// The parent Router that the Dropdown is attached to
    /// </summary>
    public RouterBuilding parentTower;
    /// <summary>
    /// If the mouse if over the Dropdowns
    /// </summary>
    /// <remarks>
    /// Used to keep the menus open when clicked on
    /// </remarks>
    //public bool Over = false;

    public RingDisplay display;

    public List<RolodexSelection> attributeSelections = new List<RolodexSelection>();

    /// <summary>
    /// Sets the RouterObject it's attached to and initializes the menu options
    /// </summary>
    private void Start()
    {
        parentTower = transform.root.gameObject.GetComponent<RouterBuilding>();
        display = transform.parent.GetComponent<RingDisplay>();
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

    private void OnEnable()
    {
        DefaultSelection();
    }

    private void DefaultSelection()
    {
        if (attributeSelections.Count <= 0)
        {
            return;
        }
        foreach (RolodexSelection selection in attributeSelections)
        {
            selection.Selected = false;
        }
        attributeSelections[0].Selected = true;
    }

    public void NextSelection()
    {
        for (int i = 0; i < attributeSelections.Count; i++)
        {
            if(attributeSelections[i].Selected)
            {
                attributeSelections[i].Selected = false;
                RolodexSelection select = attributeSelections[(i + 1) % attributeSelections.Count];
                StartCoroutine(SetSelectedAfterFrame(select));
                return;
            }
        }
    }

    IEnumerator SetSelectedAfterFrame(RolodexSelection selection)
    {
        yield return new WaitForEndOfFrame();
        selection.Selected = true;
    }

    /// <summary>
    /// Populates the Dropdown lists based on the Enums of AgentAttribute
    /// </summary>
    private void PopulateDropdowns()
    {
        List<string> colorList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.possibleColors)));
        List<string> sizeList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.possibleSizes)));
        List<string> speedList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.possibleSpeeds)));
        List<List<string>> enumList = new List<List<string>>();
        enumList.Add(colorList);
        enumList.Add(sizeList);
        enumList.Add(speedList);

        if (transform.childCount != enumList.Count + 1) //plus one for the background
        {
            Debug.LogError("Number of dropdown menus do not mirror number of attributes! Remove or add more menus!");
            return;
        }

        for (int i = 0; i < enumList.Count; i++)
        {
            Dropdown currentDropdown = transform.GetChild(i + 1).GetComponent<Dropdown>(); //plus one for the background
            if (currentDropdown == null)
            {
                Debug.LogError("Cannot find child of RoutingOptions!");
                return;
            }
            currentDropdown.onValueChanged.AddListener(delegate {
                UpdateFilter(); //only called when the Dropdown value is changed
            });
            currentDropdown.ClearOptions();
            currentDropdown.AddOptions(enumList[i]);
            if(enumList[i].Count - 1 != 3)
            {
                Debug.LogWarning("Enum list length is not as expected!");
            }
            RolodexSelection selection = currentDropdown.GetComponent<RolodexSelection>();
            selection.ChangeValue(enumList[i].Count - 1);
            attributeSelections.Add(selection);
        }
        UpdateFilter();
        DefaultSelection();

    }

    /*

    /// <summary>
    /// Basic workaround so that the menu does not close when clicked on, sets Over to true
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Over = true;
    }

    /// <summary>
    /// Basic workaround so that the menu does not close when clicked on, sets Over to false
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Over = false;
    }

    */

    /// <summary>
    /// Will set the parent Router's filter to match the one's in the Dropdown
    /// </summary>
    /// <remarks>
    /// Triggered everytime something changes in the Dropdown
    /// </remarks>
    public void UpdateFilter()
    {
        currentAttribute.Color = (AgentAttribute.possibleColors)transform.Find("Color").GetComponent<Dropdown>().value;
        currentAttribute.Size = (AgentAttribute.possibleSizes)transform.Find("Size").GetComponent<Dropdown>().value;
        currentAttribute.Speed = (AgentAttribute.possibleSpeeds)transform.Find("Speed").GetComponent<Dropdown>().value;
        if (parentTower.filter != null)
        {
            if (parentTower.filter.Count > 0)
            {
                parentTower.filter.Clear(); //change to support more than one filter later
            }
            parentTower.filter.Add(currentAttribute);
        }

        display.UpdateDisplay(currentAttribute);
    }

}
