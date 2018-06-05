using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlPrefs : MonoBehaviour {


    private Dictionary<string, KeyCode> inputDictionary = new Dictionary<string, KeyCode>()
    {
        #region Tooltips
        {"toggleTooltips", KeyCode.F1},
        #endregion

        #region RolodexSelection
        {"rolodexLeftKey", KeyCode.Q},
        {"rolodexRightKey", KeyCode.E},
        {"rolodexResetKey", KeyCode.Tab},
        {"rolodexNextKey", KeyCode.Space}
        #endregion
    };

    public void OnKeyChanged()
    {
        List<KeyCode> duplicateValues = inputDictionary.Values.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();
        if(duplicateValues.Count > 0)
        {
            Debug.LogWarning("Duplicate Key detected!");
        }
    }

    public bool GetKeyDown(string str)
    {
        return Input.GetKeyDown(inputDictionary[str]);
    }

    public KeyCode this[string str]
    {
        get
        {
            return inputDictionary[str];
        }
    }


}
