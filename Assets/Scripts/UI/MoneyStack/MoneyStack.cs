using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStack : MonoBehaviour
{

    public GameObject coinPrefab;
    public int coinPerCash = 50;
    public float updateTime = 0.1f;
    public float moveUpBy = 10f;

    public static bool Modifying = false;
    public static int coinCount = 0;

    void Start()
    {
        if (coinPrefab.tag != "Coin")
        {
            Debug.LogError("Please place a valid Coin object in this script. (Must have the 'Coin' tag)");
            return;
        }

    }

    private void Update()
    {
        if (Modifying || Score.score < 0)
        {
            return;
        }
        int desireValue = (int)Score.score / coinPerCash;
        int toModify = desireValue - (transform.childCount); //1 accounts for bottom
        if (Mathf.Abs(toModify) > 0)
        {
            StartCoroutine(CreateCoins(toModify));
        }
    }

    private void CreateCoin()
    {
        GameObject coin = Instantiate(coinPrefab, transform.parent);
        transform.position += Vector3.up * moveUpBy;
        coin.transform.SetParent(transform);
        coin.transform.SetAsFirstSibling();
    }

    private void DeleteCoin()
    {
        transform.position += Vector3.up * -moveUpBy;
        Destroy(transform.GetChild(0).gameObject);
    }

    IEnumerator CreateCoins(int count)
    {
        Modifying = true;
        int absCount = Mathf.Abs(count);
        for (int i = 0; i < absCount; i++)
        {
            if (count > 0)
            {
                CreateCoin();
            } else
            {
                DeleteCoin();
            }
            yield return new WaitForSeconds(updateTime);
        }
        Modifying = false;
    }



}
