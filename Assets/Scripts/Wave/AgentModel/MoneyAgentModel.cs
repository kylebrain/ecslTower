using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyAgentModel : AgentModel {

    private float lightRangeMod = 2f;
    private float rangeWeight = 2f;

    public override void SetColor(Color color)
    {
        Light light = transform.Find("Light").GetComponent<Light>();
        light.color = color;
    }

    public override void SetSize(float size)
    {
        if (size < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 0.5f);
        }
        else
        {
            transform.localScale = new Vector3(0.5f * size, 1f, 0.5f * size);
            Light light = transform.Find("Light").GetComponent<Light>();
            light.range = lightRangeMod * Mathf.Pow(size, 1 / rangeWeight);
        }
    }
}
