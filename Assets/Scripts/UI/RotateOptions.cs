using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class RotateOptions : MonoBehaviour
{

    private Dropdown dropdown;

    public bool selected;

    public Text text1;
    public Text text2;
    public Text text3;

    private List<Text> textList = new List<Text>();
    private float spacing;
    private Text textDisplayed;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        textDisplayed = text2;
        textList.Add(text1);
        textList.Add(text2);
        textList.Add(text3);
        spacing = dropdown.GetComponent<RectTransform>().sizeDelta.x;
        textDisplayed.text = dropdown.options[1].text;
        dropdown.value = 1;

    }

    private void Update()
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeDropdown(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeDropdown(1);
            }
        }
    }

    //figure out which direction is being moved
    //place the other text in that direction and then move to the next
    //set the textDisplay to the new text value
    private void ChangeDropdown(int direction)
    {
        if(direction != 1 && direction != -1)
        {
            Debug.LogError("Direction must be 1 or -1!");
            return;
        }
        string nextText;
        int index = (dropdown.value + direction) % dropdown.options.Count;
        if(index < 0)
        {
            index = dropdown.options.Count - 1;
        }
        dropdown.value = index;
        nextText = dropdown.options[index].text;

        textList[1 + direction].text = nextText;

        textList[1].GetComponent<RotateText>().TransitionToVector(new Vector3(direction * -spacing, 0f));
        textList[1 + direction].GetComponent<RotateText>().TransitionToVector(Vector3.zero);

        textList[1 - direction].transform.localPosition = new Vector3(direction * spacing, 0f);
        textList[1 - direction].GetComponent<RotateText>().TransitionToVector(new Vector3(direction * spacing, 0f)); //need to set target to keep in position

        Text temp = textList[1 - direction];
        textList.RemoveAt(1 - direction);
        textList.Insert(1 + direction, temp);

        textDisplayed = textList[1];

    }

}
