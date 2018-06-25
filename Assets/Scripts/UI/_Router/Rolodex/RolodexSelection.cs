using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Dropdown))]
[RequireComponent(typeof(RectMask2D))]
public class RolodexSelection : MonoBehaviour
{
    private Dropdown dropdown;

    private Color selectedColor;
    private Color deselectedColor;
    private Color disabledColor;
    public float shiftSpeed = 5f;
    public float fudge = 0.001f;

    public UnityEvent OnNext = new UnityEvent();
    public UnityEvent OnPrevious = new UnityEvent();

    public RolodexArrow[] arrowArray = new RolodexArrow[2];

    private ControlPrefs controlPrefs;

    private VisualPrefs visualPrefs;
    private Graphic image;

    public bool Disabled
    {
        get
        {
            return disabled;
        }
        set
        {
            if (value)
            {
                if (selected)
                {
                    OnNext.Invoke();
                }
                image.color = disabledColor;
            }
            else
            {
                //if this is the only enabled selection, select this
                image.color = deselectedColor;
            }

            for (int i = 0; i < arrowArray.Length; i++)
            {
                arrowArray[i].Disabled(value);
            }
            disabled = value;
        }
    }
    private bool disabled;

    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            if (Disabled)
            {
                selected = false;
                return;
            }
            if (value)
            {
                image.color = selectedColor;
            } else
            {
                image.color = deselectedColor;
            }
            selected = value;
        }
    }
    private bool selected;

    public RolodexText text1;
    public RolodexText text2;
    public RolodexText text3;

    private List<RolodexText> textList = new List<RolodexText>();
    private float spacing;
    private RolodexText currentText;

    private void Awake()
    {
        controlPrefs = GameObject.FindGameObjectWithTag("ControlPrefs").GetComponent<ControlPrefs>();
        if(controlPrefs == null)
        {
            Debug.LogError("Cannot find ControlPrefs, perhaps it was moved or the tag was not applied?");
            return;
        }

        visualPrefs = GameObject.Find("VisualPrefs").GetComponent<VisualPrefs>();
        if (visualPrefs == null)
        {
            Debug.LogError("Cannot find VisualPrefs, perhaps it was moved or renamed?");
            return;
        }
        selectedColor = visualPrefs.selectedColor;
        deselectedColor = visualPrefs.deselectedColor;
        disabledColor = visualPrefs.disabledColor;

        image = GetComponent<Graphic>();

        arrowArray[0] = transform.Find("LeftArrow").GetComponent<RolodexArrow>();
        arrowArray[1] = transform.Find("RightArrow").GetComponent<RolodexArrow>();

        Selected = selected; //initialize the color

        if (text1 == null || text2 == null || text3 == null)
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

        for (int i = 0; i < textList.Count; i++)
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

        if (OnNext.GetPersistentEventCount() <= 0 && OnPrevious.GetPersistentEventCount() <= 0)
        {
            Debug.LogWarning("Rolodex does not have next or previous handling implemented!");
        }

    }

    private void Update()
    {
        if (Selected)
        {
            for (int i = 1; i < dropdown.options.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    ChangeValue(i - 1);
                    OnNext.Invoke();
                    break;
                }
            }

            if(controlPrefs.GetKeyDown("rolodexResetKey"))
            {
                ChangeValue(dropdown.options.Count - 1);
                OnNext.Invoke();
            }

            if (controlPrefs.GetKeyDown("rolodexLeftKey"))
            {
                ChangeDropdown(false);
            }
            else if (controlPrefs.GetKeyDown("rolodexRightKey"))
            {
                ChangeDropdown(true);
            }
            else if (controlPrefs.GetKeyDown("rolodexNextKey"))
            {
                OnNext.Invoke();
            }
        }
    }

    public void ChangeValue(int destination)
    {
        int origin = dropdown.value;
        if (origin == destination)
        {
            return;
        }
        int count = dropdown.options.Count;
        if (destination >= count || destination < 0)
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
        if (Mathf.Abs(path1) <= Mathf.Abs(path2))
        {
            right = path1 > 0;
            numberShifted = Mathf.Abs(path1);
        }
        else
        {
            right = path2 > 0;
            numberShifted = Mathf.Abs(path2);
        }

        for (int i = 0; i < numberShifted; i++)
        {
            ChangeDropdown(right);
        }

    }

    //figure out which direction is being moved
    //place the other text in that direction and then move to the next
    //set the textDisplay to the new text value
    public void ChangeDropdown(bool right)
    {
        int direction;
        if (right)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        string nextText;
        int index = (dropdown.value + direction) % dropdown.options.Count;
        if (index < 0)
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

    public void Hover(bool isHover)
    {
        if(!Disabled)
            image.color = (isHover || Selected) ? selectedColor : deselectedColor;
    }

}
