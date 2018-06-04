using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
[RequireComponent(typeof(RectMask2D))]
public class RolodexSelection : MonoBehaviour
{

    private Dropdown dropdown;

    public bool selected;

    public float shiftSpeed = 5f;
    public float fudge = 0.001f;

    public RolodexText text1;
    public RolodexText text2;
    public RolodexText text3;

    private List<RolodexText> textList = new List<RolodexText>();
    private float spacing;
    private RolodexText currentText;

    private void Awake()
    {
        if(text1 == null || text2 == null || text3 == null)
        {
            Debug.LogError("Rolodex selection box must have three text boxes with the RolodexText component");
            enabled = false;
            return;
        }

        //cleanup and add null detection for text and automatic scaling for the text

        dropdown = GetComponent<Dropdown>();

        textList.Add(text1);
        textList.Add(text2);
        textList.Add(text3);
        spacing = dropdown.GetComponent<RectTransform>().sizeDelta.x;

        for(int i = 0; i < textList.Count; i++)
        {
            RectTransform rectTransform = textList[i].GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(spacing, rectTransform.sizeDelta.y);
            rectTransform.localPosition = new Vector2((i - 1) * spacing, 0f);
            textList[i].TransitionToVector(new Vector3((i - 1) * spacing, 0f));
        }

        //set the current text box to the first value
        currentText = text2;
        currentText.GetComponent<Text>().text = dropdown.options[0].text;
        dropdown.value = 0;

    }


    private void Update()
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeDropdown(false);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeDropdown(true);
            } else if (Input.GetKeyDown(KeyCode.Tab))
            {
                transform.parent.GetComponent<RoutingOptions>().NextSelection();
                Debug.Log("Next selection!", this);
            }
        }
    }

    public void ChangeValue(int destination)
    {
        int origin = dropdown.value;
        if(origin == destination)
        {
            return;
        }
        int count = dropdown.options.Count;
        if(destination >= count || destination < 0)
        {
            Debug.LogError("Destination must be set to valid option!");
            return;
        }
        //both paths have a direction, positive is right
        int path1 = destination - origin;
        int path2 = -count * (int)Mathf.Sign(path1) - origin + destination; //counts the number of steps to go the opposite way of path1
        bool right;
        int numberShifted;
        //takes the shortest path, if equal it chooses the direct path
        if(Mathf.Abs(path1) <= Mathf.Abs(path2))
        {
            right = path1 > 0;
            numberShifted = Mathf.Abs(path1);
        } else
        {
            right = path2 > 0;
            numberShifted = Mathf.Abs(path2);
        }

        for(int i = 0; i < numberShifted; i++)
        {
            ChangeDropdown(right);
        }

    }

    //figure out which direction is being moved
    //place the other text in that direction and then move to the next
    //set the textDisplay to the new text value
    private void ChangeDropdown(bool right)
    {
        int direction;
        if (right)
        {
            direction = 1;
        } else
        {
            direction = -1;
        }
        string nextText;
        int index = (dropdown.value + direction) % dropdown.options.Count;
        if(index < 0)
        {
            index = dropdown.options.Count - 1;
        }
        dropdown.value = index;
        nextText = dropdown.options[index].text;

        textList[1 + direction].GetComponent<Text>().text = nextText;

        textList[1].GetComponent<RolodexText>().TransitionToVector(new Vector3(direction * -spacing, 0f));
        textList[1 + direction].GetComponent<RolodexText>().TransitionToVector(Vector3.zero);

        textList[1 - direction].transform.localPosition = new Vector3(direction * spacing, 0f);
        textList[1 - direction].GetComponent<RolodexText>().TransitionToVector(new Vector3(direction * spacing, 0f)); //need to set target to keep in position

        RolodexText temp = textList[1 - direction];
        textList.RemoveAt(1 - direction);
        textList.Insert(1 + direction, temp);

        currentText = textList[1];

    }

}
