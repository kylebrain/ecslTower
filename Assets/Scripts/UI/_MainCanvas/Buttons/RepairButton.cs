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

    protected override void DerivedStart()
    {
        Rebuilding = false;
        rebuildCost = (int)(repairCost * ((float)Score.MaxHealth / healthRepaired));
        SetEnable(false);
    }

    private void Update()
    {
        if (Score.Health <= 0)
        {
            GetText.text = "Rebuild: $" + rebuildCost;
        }
        else if (!Rebuilding)
        {
            GetText.text = "Repair: $" + repairCost;
        }

        if (!Rebuilding)
        {
            if (Score.Health < Score.MaxHealth && Score.Money >= repairCost && !ButtonEnabled)
            {
                SetEnable(true);
            }
            if (Score.Health >= Score.MaxHealth || Score.Money < repairCost && ButtonEnabled)
            {
                SetEnable(false);
            }
            if (Score.Health <= 0 && Score.Money < rebuildCost)
            {
                Score.GameLost();
            }
        }
        

    }

    private void Repair(int cost)
    {
        if (Score.Money >= cost)
        {
            AudioManager.Play("Repair", 0.1f);
            Score.Health += healthRepaired;
            Score.Money -= cost;
        }
    }

    private void CheckRebuild()
    {
        if (Score.Money >= rebuildCost)
        {
            StartCoroutine(Rebuild());
        }
    }

    IEnumerator Rebuild()
    {
        AudioManager.Play("Repair");
        Score.Money -= rebuildCost;
        Rebuilding = true;
        SetEnable(false);
        while (Score.Health != Score.MaxHealth)
        {
            yield return new WaitForSeconds(rebuildTime);
            Repair(0);
        }
        Rebuilding = false;
        SetEnable(true);
    }

    public override void PerformAction()
    {
        if (Score.Health <= 0)
        {
            CheckRebuild();
        }
        else
        {
            Repair(repairCost);
        }

    }


}
