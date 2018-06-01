using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sell: GameButton {
    private RouterBuilding root;

    private void Start()
    {
        root = transform.root.GetComponent<RouterBuilding>();
        transform.Find("Button").Find("Text").GetComponent<Text>().text = "Sell: $" + root.sellPrice;
    }

    public override void PerformAction() {

        int returnMoney = root.sellPrice;
        Score.score += returnMoney;

        root.removeFromMap();
    }
}
