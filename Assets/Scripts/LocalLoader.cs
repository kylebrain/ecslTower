using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalLoader : MonoBehaviour {

	public void LoadLocally(string sceneName)
    {
        SceneLoader.LoadScene(sceneName);
    }
}
