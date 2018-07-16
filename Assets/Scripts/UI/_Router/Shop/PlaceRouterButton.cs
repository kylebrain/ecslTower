using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlaceRouterButton : DisableButton {
    public Building RouterPrefab;
    public string hotkeyString;
    public Text shopText;
    private int count = 0;
    private string buildingName;

    protected override void DerivedStart()
    {
        buildingName = RouterPrefab.GetType().ToString().Replace("Building", "");
        GetText.text = "$" + RouterPrefab.price;
        shopText.text = "Buy " + buildingName +  " [" + ControlPrefs.GetKey(hotkeyString) + "]";
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
        if (ControlPrefs.GetKeyDown(hotkeyString))
        {
            PerformAction();
        }
        UpdateButton();
    }

    public override void PerformAction() {
        if (Score.Money >= RouterPrefab.price &&  !Building.currentlyPlacing && GetButton.interactable)
        {
            Score.Money -= RouterPrefab.price;
            // play a buying sound
            //Building newBuilding = Instantiate(RouterPrefab);
            //newBuilding.transform.position = new Vector3(int.MinValue, RouterPrefab.transform.position.y, int.MinValue);
            //newBuilding.name = buildingName + " " + count;
            //count++;
            //NetworkIdentity identity = newBuilding.GetComponent<NetworkIdentity>();

            //spawned here, object is valid
            transform.root.GetComponent<Defender>().CmdSpawnBuilding(RouterPrefab.name);
            //newBuilding.name = buildingName + " " + count;
            count++;
        }
    }
}
