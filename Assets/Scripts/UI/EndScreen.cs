using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour {

    private float previousAudioVolume;

    public void EndGame(bool won, int score = 0)
    {
        previousAudioVolume = AudioListener.volume;
        AudioListener.volume = 0; //or find another way to turn off the game sfx maybe with an audio mixer?
        if (won)
        {
            transform.Find("Score").GetComponent<Text>().text = "Score: " + score;
        } else
        {
            transform.Find("Score").GetComponent<Text>().text = "You were destroyed!";
        }
        //set canvas to the top
        transform.parent.SetAsLastSibling();
        gameObject.SetActive(true);
    }

	// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf && Input.anyKeyDown)
        {
            AudioListener.volume = previousAudioVolume;
            SceneManager.LoadScene("LevelSelect");
        }
	}
}
