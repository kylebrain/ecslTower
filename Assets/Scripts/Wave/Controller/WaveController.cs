using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    private WaveManager waveManager;

	void Awake () {
        waveManager = GameObject.FindWithTag("WaveManager").GetComponent<WaveManager>();
        if (waveManager == null)
        {
            Debug.LogError("Could not find WaveManager object in the scene. Either the tag was changed or the object is missing.");
        }
    }
	
}
