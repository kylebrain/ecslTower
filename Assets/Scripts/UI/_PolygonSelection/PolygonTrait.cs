using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonTrait : MonoBehaviour {

    public RingDisplayAgent displayAgent;

    [SerializeField]
    public PolygonSelection[] traits;

    public AgentAttribute currentAttribute
    {
        get
        {
            AgentAttribute ret;
            ret.Color = (AgentAttribute.PossibleColors)traits[0].Selection;
            ret.Size = (AgentAttribute.PossibleSizes)traits[1].Selection;
            ret.Speed = (AgentAttribute.PossibleSpeeds)traits[2].Selection;
            return ret;
        }
    }

    private void Awake()
    {
        List<List<string>> namesList = new List<List<string>>
        { System.Enum.GetNames(typeof(AgentAttribute.PossibleColors)).ToList(),
        System.Enum.GetNames(typeof(AgentAttribute.PossibleSizes)).ToList(),
        System.Enum.GetNames(typeof(AgentAttribute.PossibleSpeeds)).ToList()};

        if(traits.Length != namesList.Count)
        {
            Debug.LogError("Must have the same number of PolygonSelections as Enums!\nCurrently: " + namesList.Count);
            return;
        }

        int index = 0;
        foreach (List<string> names in namesList)
        {
            names.Remove("dontCare");
            for (int i = 0; i < names.Count; i++)
            {
                names[i] = char.ToUpper(names[i][0]) + names[i].Substring(1);
            }
            traits[index].Initialize(names.ToArray());
            index++;
        }

        UpdateAttribute();
    }

    public void UpdateAttribute()
    {
        displayAgent.InitializeAttributes(currentAttribute);
    }
}
