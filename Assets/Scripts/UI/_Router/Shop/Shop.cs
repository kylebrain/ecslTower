using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop: GameButton {
    public ShopContainer shopContainer;

    public override void PerformAction() {
        if(shopContainer == null) {
            Debug.LogError("Could not find 'Shop Contents'. Make sure it exists in the scene.");
        }

        shopContainer.showAll();
        Hide();
    }
}
