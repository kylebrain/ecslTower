using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sell: GameButton {
    private RouterBuilding root;
    private int currentPrice; 

    private void Start()
    {
        root = transform.root.GetComponent<RouterBuilding>();
        currentPrice = root.price;
        GetText.text = "[" + ControlPrefs.GetKey("sellRouter") + "] Return: $" + currentPrice;
        StartCoroutine(waitForPlace());
    }

    private void Update()
    {
        if(GetButton.interactable && ControlPrefs.GetKeyDown("sellRouter"))
        {
            PerformAction();
        }
    }

    public override void PerformAction() {
        Score.Money += currentPrice;
        root.removeFromMap();
        Building.currentlyPlacing = false; //if you are able to select a building when placing one this is wrong, currently works
    }

    IEnumerator waitForPlace()
    {
        yield return new WaitUntil(() => root.Placed);
        currentPrice = root.sellPrice;
        GetText.text = "[" + ControlPrefs.GetKey("sellRouter") + "] Sell: $" + currentPrice;
    }
}
