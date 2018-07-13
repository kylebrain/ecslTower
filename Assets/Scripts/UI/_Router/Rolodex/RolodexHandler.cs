using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the Dropdown menus of the Router to select what to filter
/// </summary>
public class RolodexHandler : MonoBehaviour // , IPointerEnterHandler, IPointerExitHandler
{
    public bool includeDontCare = true;

    /// <summary>
    /// The AgentAttribute that the Dropdowns currently represent
    /// </summary>
    public AgentAttribute currentAttribute;
    
    /// <summary>
    /// If the mouse if over the Dropdowns
    /// </summary>
    /// <remarks>
    /// Used to keep the menus open when clicked on
    /// </remarks>
    //public bool Over = false;

    private RingDisplay display;

    public List<RolodexSelection> attributeSelections = new List<RolodexSelection>();

    /// <summary>
    /// Sets the RouterObject it's attached to and initializes the menu options
    /// </summary>
    private void Start()
    {
        
        display = transform.parent.GetComponent<RingDisplay>();
        PopulateDropdowns();
        DerivedStart();

    }

    protected virtual void DerivedStart() { }

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

    public void CheckEnable()
    {
        int disabledCount = 0;
        int selectedCount = 0;
        for (int i = 0; i < attributeSelections.Count; i++)
        {
            if (attributeSelections[i].Disabled)
            {
                disabledCount++;
            }
            if (attributeSelections[i].Selected)
            {
                selectedCount++;
            }
        }

        if(selectedCount == 0 && disabledCount < attributeSelections.Count)
        {
            for (int i = 0; i < attributeSelections.Count; i++)
            {
                if (!attributeSelections[i].Disabled)
                {
                    attributeSelections[i].Selected = true;
                    break;
                }
            }
        }

    }

    public void NextSelection()
    {

        RolodexSelection currentSelection = attributeSelections[0];
        int currentIndex = 0;
        for (int i = 0; i < attributeSelections.Count; i++)
        {
            if (attributeSelections[i].Selected)
            {
                currentSelection = attributeSelections[i];
                currentIndex = i;
                break;
            }
        }

        int index = currentIndex;
        RolodexSelection indexSelection = null;
        while((indexSelection = attributeSelections[(++index) % attributeSelections.Count]) != currentSelection)
        {
            if(!indexSelection.Disabled)
            {
                currentSelection.Selected = false;
                StartCoroutine(SetSelectedAfterFrame(indexSelection));
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
        List<string> colorList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.PossibleColors)));
        List<string> sizeList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.PossibleSizes)));
        List<string> speedList = new List<string>(System.Enum.GetNames(typeof(AgentAttribute.PossibleSpeeds)));
        List<List<string>> enumList = new List<List<string>>();
        enumList.Add(colorList);
        enumList.Add(sizeList);
        enumList.Add(speedList);

        if (!includeDontCare)
        {
            foreach (List<string> stringList in enumList)
            {
                stringList.RemoveAt(stringList.Count - 1);
            }
        }

        if (transform.childCount != enumList.Count + 1) //plus one for the background
        {
            Debug.LogError("Number of dropdown menus do not mirror number of attributes! Remove or add more menus!");
            return;
        }

        //this should account for the panel, but if it needs to be completely removed, there might be an off by one below

        int panelInt = 1;

        Transform panel = transform.Find("Panel");
        if(panel == null)
        {
            Debug.LogError("Could not find Panel object, perhaps it was moved or renamed!");
            panelInt = 0;
        } else if(panel.GetSiblingIndex() != 0)
        {
            Debug.LogError("The Panel object is not where it should be! Place it as the first child!");
            return;
        }

        for (int i = 0; i < enumList.Count; i++)
        {
            Dropdown currentDropdown = transform.GetChild(i + panelInt).GetComponent<Dropdown>(); //plus one for the panel
            if (currentDropdown == null)
            {
                Debug.LogError("Cannot find child of RoutingOptions!");
                return;
            }
            currentDropdown.onValueChanged.AddListener(delegate {
                UpdateFilter(); //only called when the Dropdown value is changed
            });

            //capitalizes each enum
            for(int j = 0; j < enumList[i].Count; j++)
            {
                string str = enumList[i][j];
                enumList[i][j] = char.ToUpper(str[0]) + str.Substring(1);
            }

            currentDropdown.ClearOptions();
            currentDropdown.AddOptions(enumList[i]);
            if(currentDropdown.options[currentDropdown.options.Count-1].text == "DontCare")
            {
                currentDropdown.options[currentDropdown.options.Count - 1].text = "All";
            } /* else
            {
                Debug.LogError("Last enum value should be 'DontCare.' Perhaps it was moved or renamed?");
            }

            if(enumList[i].Count - 1 != 3)
            {
                Debug.LogWarning("Enum list length is not as expected!");
            }
            */
            RolodexSelection selection = currentDropdown.GetComponent<RolodexSelection>();
            if(includeDontCare)
            {
                selection.ChangeValue(enumList[i].Count - 1);
            } else
            {
                selection.ChangeValue(0);
            }
            
            attributeSelections.Add(selection);
        }
        UpdateFilter();
        DefaultSelection();

    }

    /// <summary>
    /// Will set the parent Router's filter to match the one's in the Dropdown
    /// </summary>
    /// <remarks>
    /// Triggered everytime something changes in the Dropdown
    /// </remarks>
    public void UpdateFilter()
    {
        currentAttribute.Color = (AgentAttribute.PossibleColors)transform.Find("Color").GetComponent<Dropdown>().value;
        currentAttribute.Size = (AgentAttribute.PossibleSizes)transform.Find("Size").GetComponent<Dropdown>().value;
        currentAttribute.Speed = (AgentAttribute.PossibleSpeeds)transform.Find("Speed").GetComponent<Dropdown>().value;
        
        display.UpdateDisplay(currentAttribute);
        DerivedUpdateFilter(currentAttribute);
        
    }

    protected virtual void DerivedUpdateFilter(AgentAttribute agentAttribute) { }

    public void DisableSelection(int index, bool disable = true)
    {
        if(index >= attributeSelections.Count)
        {
            throw new System.ArgumentOutOfRangeException();
        }
        if(index == -1)
        {
            for(int i = 0; i < attributeSelections.Count; i++)
            {
                attributeSelections[i].Disabled = disable;
            }
        } else
        {
            attributeSelections[index].Disabled = disable;
        }
    }

}
