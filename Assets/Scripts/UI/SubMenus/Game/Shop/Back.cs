using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back: GameButton {
    public GameButton shop;

    public override void PerformAction() {
        transform.GetComponentInParent<ShopContainer>().hideAll();
        shop.Show();
    }
}
