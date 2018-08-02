using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonButton))]
public class CurrencyPolygonButton : MonoBehaviour {

	public void BuyMutate()
    {
        if(Attacker.HackerCurrency >= Attacker.MutateCost)
        {
            Attacker.HackerCurrency -= Attacker.MutateCost;
            GetComponent<PolygonButton>().MoveTraveler();
        }
        else
        {
            Debug.LogWarning("Not enough money to mutate!");
        }
    }
}
