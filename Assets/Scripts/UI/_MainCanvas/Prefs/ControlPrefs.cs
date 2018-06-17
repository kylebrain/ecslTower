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

        #region Camera
        {"toggleCameraMode", KeyCode.R},
        {"adjustCameraAngle", KeyCode.F },
        #endregion

        #region RolodexSelection
        {"rolodexLeftKey", KeyCode.Q},
        {"rolodexRightKey", KeyCode.E},
        {"rolodexResetKey", KeyCode.Tab},
        {"rolodexNextKey", KeyCode.Space}
        #endregion
    };

    private void Awake()
    {
        foreach(string str in inputDictionary.Keys.ToList())
        {
            if (!PlayerPrefs.HasKey(str))
            {
                continue;
            }
            //gets the PlayerPref, defaults to itself if it doesn't find it, then parses to a KeyCode
            inputDictionary[str] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(str, inputDictionary[str].ToString()));
        }

    }

    //why did I create a dictionary of Keycodes :(
    /// <summary>
    /// Updates the inputDictionary when an option is changed
    /// </summary>
    /// <param name="keyLookupString">Dictionary key to change</param>
    /// <param name="newKeyCodeKeyCode">Value will be changed to this KeyCode</param>
    /// <returns>returns number of duplicates</returns>
    public int OnKeyChanged(string keyLookupString, KeyCode newKeyCodeKeyCode)
    {
        //use below if you need to pass in a KeyCode using an input box

        /*KeyCode newKeyCodeKeyCode;
        //is the second arguement a valid KeyCode?
        try
        {
            newKeyCodeKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), newKeyCodeString);
        }
        catch(System.Exception e)
        {
            string thisisonlyheretogetridoftheunusedwarning = e.Message;
            throw new System.ArgumentException("Passed KeyCode string is not valid.", "newKeyCodeString");
        }*/

        //is the first arguement a valid lookup key?
        KeyCode currentKeyCode;

        if(inputDictionary.TryGetValue(keyLookupString, out currentKeyCode))
        {
            if(newKeyCodeKeyCode == currentKeyCode)
            {
                Debug.LogWarning("Values are the same, are you call this function when key is changed?");
                return 0;
            }

            //key lookup and new KeyCode are valid!

            inputDictionary[keyLookupString] = newKeyCodeKeyCode; //changed the value
            PlayerPrefs.SetString(keyLookupString, newKeyCodeKeyCode.ToString()); //add it to the preferences


        } else
        {
            throw new System.ArgumentException("Passed lookup key string is not valid.", "keyLookupString");
        }


        //checking for duplicates
        List<KeyCode> duplicateValues = inputDictionary.Values.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();
        if(duplicateValues.Count > 0)
        {
            Debug.LogWarning("Duplicate Key detected!");
            return duplicateValues.Count;
        }

        return 0;
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
