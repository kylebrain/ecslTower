using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingScript : MonoBehaviour {

    public int periodCount = 3;
    public float transitionTime = 1f;

    private void OnEnable()
    {
        StartCoroutine(AnimatedText());
    }

    private void OnDisable()
    {
        StopCoroutine(AnimatedText());
    }

    IEnumerator AnimatedText()
    {
        Text text = GetComponent<Text>();
        while (true)
        {
            text.text = "Connecting";
            for (int i = 0; i < periodCount + 1; i++)
            {
                yield return new WaitForSeconds(transitionTime);
                text.text += ".";
            }
        }
    }
}
