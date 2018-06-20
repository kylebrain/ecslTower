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
        transform.Find("Button").Find("Text").GetComponent<Text>().text = "Return: $" + currentPrice;
        StartCoroutine(waitForPlace());
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
        transform.Find("Button").Find("Text").GetComponent<Text>().text = "Sell: $" + currentPrice;
    }
}
