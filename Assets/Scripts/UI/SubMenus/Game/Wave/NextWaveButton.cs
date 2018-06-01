using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NextWaveButton : DisableButton
{

    public WaveController waveController;

    public override void PerformAction()
    {
        if(!waveController.Playing){
            waveController.PlayWave(this);
            SetEnable(false);
        }
    }
}
