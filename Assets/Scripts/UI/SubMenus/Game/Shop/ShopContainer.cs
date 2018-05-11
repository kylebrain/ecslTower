using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopContainer: MonoBehaviour {


    public void showAll() {
        foreach(GameButton cur in GetComponentsInChildren<GameButton>(true)) {
            cur.Show();
        }
    }

    public void hideAll() {
        foreach(GameButton cur in GetComponentsInChildren<GameButton>(true)) {
            cur.Hide();
        }
    }
}
