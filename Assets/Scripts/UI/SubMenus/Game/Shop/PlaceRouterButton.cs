using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRouterButton: GameButton {
    public Building RouterPrefab;

    private int count = 0;

    public override void PerformAction() {
        Building newBuilding = Instantiate(RouterPrefab);
        newBuilding.transform.position = new Vector3(int.MinValue, RouterPrefab.transform.position.y, int.MinValue);
        newBuilding.name = "Test Router " + count;
        count++;
    }
}
