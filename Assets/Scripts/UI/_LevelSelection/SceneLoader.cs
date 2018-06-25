using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

    public GameObject loadScreen;
    public Slider slider;
    public Text text;

    private static SceneLoader instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void LoadScene(string sceneName)
    {
        instance.StartCoroutine(instance.LoadWithScreen(sceneName));
    }

    IEnumerator LoadWithScreen(string sceneName)
    {
        slider.value = 0;
        text.text = "0%";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadScreen.SetActive(true);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            text.text = progress * 100f + "%";
            yield return null;
        }
        loadScreen.SetActive(false);
    }
}
