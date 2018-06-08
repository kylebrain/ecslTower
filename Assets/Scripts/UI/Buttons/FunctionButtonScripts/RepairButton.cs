using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairButton : DisableButton
{

    private int repairCost = 50;
    private int healthRepaired = 10;
    private float rebuildTime = 2; //time to rebuild each segment

    private int rebuildCost;
    public static bool Rebuilding = false;

    public Health healthReference;

    protected override void DerivedStart()
    {
        rebuildCost = (int)(repairCost * ((float)Health.maxHealth / healthRepaired));
        SetEnable(false);
    }

    private void Update()
    {
        if (Health.health <= 0)
        {
            GetText.text = "Rebuild: $" + rebuildCost;
        }
        else if (!Rebuilding)
        {
            GetText.text = "Repair: $" + repairCost;
        }

        if (!Rebuilding)
        {
            if (Health.health < Health.maxHealth && Score.score >= repairCost && !ButtonEnabled)
            {
                SetEnable(true);
            }
            if (Health.health >= Health.maxHealth || Score.score < repairCost && ButtonEnabled)
            {
                SetEnable(false);
            }
            if (Health.health <= 0 && Score.score < rebuildCost)
            {
                healthReference.GameLost();
            }
        }
        

    }

    private void Repair(int cost)
    {
        if (Score.score >= cost)
        {
            Health.health += healthRepaired;
            Score.score -= cost;
        }
    }

    private void CheckRebuild()
    {
        if (Score.score >= rebuildCost)
        {
            StartCoroutine(Rebuild());
        }
    }

    IEnumerator Rebuild()
    {
        Score.score -= rebuildCost;
        Rebuilding = true;
        SetEnable(false);
        while (Health.health != Health.maxHealth)
        {
            yield return new WaitForSeconds(rebuildTime);
            Repair(0);
        }
        Rebuilding = false;
        SetEnable(true);
    }

    public override void PerformAction()
    {
        if (Health.health <= 0)
        {
            CheckRebuild();
        }
        else
        {
            Repair(repairCost);
        }

    }
}
