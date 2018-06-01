using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRouterButton : DisableButton {
    public Building RouterPrefab;

    private int count = 0;

    protected override void DerivedStart()
    {
        GetText.text = "Router: $" + RouterPrefab.price;
        UpdateButton();
    }

    public override void Show()
    {
        UpdateButton();
        gameObject.SetActive(true);
    }

    private void UpdateButton()
    {
        if (!ButtonEnabled && Score.score >= RouterPrefab.price)
        {
            SetEnable(true);
        }
        if (ButtonEnabled && Score.score < RouterPrefab.price)
        {
            SetEnable(false);
        }
    }
    private void Update()
    {
        UpdateButton();
    }

    public override void PerformAction() {
        if (Score.score >= RouterPrefab.price)
        {
            Score.score -= RouterPrefab.price;
            //play a buying sound
            Building newBuilding = Instantiate(RouterPrefab);
            newBuilding.transform.position = new Vector3(int.MinValue, RouterPrefab.transform.position.y, int.MinValue);
            newBuilding.name = "Test Router " + count;
            count++;
        }
    }
}
