using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceRouterButton : DisableButton {
    public Building RouterPrefab;

    public Text shopText;
    private int count = 0;

    protected override void DerivedStart()
    {
        GetText.text = "$" + RouterPrefab.price;
        shopText.text = "Buy Router [" + ControlPrefs.GetKey("buyRouter") + "]";
        UpdateButton();
    }

    public override void Show()
    {
        UpdateButton();
        gameObject.SetActive(true);
    }

    private void UpdateButton()
    {
        if (!ButtonEnabled && Score.Money >= RouterPrefab.price)
        {
            SetEnable(true);
        }
        if (ButtonEnabled && Score.Money < RouterPrefab.price)
        {
            SetEnable(false);
        }
    }
    private void Update()
    {
        if (ControlPrefs.GetKeyDown("buyRouter"))
        {
            PerformAction();
        }
        UpdateButton();
    }

    public override void PerformAction() {
        if (Score.Money >= RouterPrefab.price &&  !Building.currentlyPlacing && GetButton.interactable)
        {
            Score.Money -= RouterPrefab.price;
            //play a buying sound
            Building newBuilding = Instantiate(RouterPrefab);
            //newBuilding.transform.position = new Vector3(int.MinValue, RouterPrefab.transform.position.y, int.MinValue);
            newBuilding.name = "Router " + count;
            count++;
        }
    }
}
