using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sell: GameButton {
    private RouterBuilding routerBuilding;

    public override void PerformAction() {
        RouterBuilding root = transform.root.gameObject.GetComponent<RouterBuilding>();

        //float returnMoney = root.sellPrice;
        //use above to increase functionality
        //TODO: Give the player the money from selling this tower


        root.removeFromMap();
    }
}
