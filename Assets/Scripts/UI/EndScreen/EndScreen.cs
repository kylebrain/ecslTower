using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour {

    private float previousAudioVolume;
    private AudioSource winAudio;
    private AudioSource loseAudio; //make sure to change this sound to reflect the malicious agent sound
    private GameObject screen;

    private void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        winAudio = audioSources[0];
        loseAudio = audioSources[1];

        screen = transform.Find("EndScreen").gameObject;
    }

    public void EndGame(bool won, int score = 0)
    {
        if (won)
        {
            screen.transform.Find("Score").GetComponent<Text>().text = "Score: " + score;
            winAudio.Play();
            StartCoroutine(DisplayEnd(winAudio.clip.length)); //plays the sound then waits to show screen
        } else
        {
            screen.transform.Find("Score").GetComponent<Text>().text = "You were destroyed!";
            StartCoroutine(DisplayEnd(loseAudio.clip.length)); //waits for the final explosion before showing the screen
        }
        
    }

	// Update is called once per frame
	void Update () {
        if (screen.activeSelf && Input.anyKeyDown)
        {
            AudioListener.volume = previousAudioVolume;
            SceneManager.LoadScene("LevelSelect");
        }
	}

    private void Display()
    {
        previousAudioVolume = AudioListener.volume;
        AudioListener.volume = 0; //or find another way to turn off the game sfx maybe with an audio mixer?
        screen.SetActive(true);
    }

    IEnumerator DisplayEnd(float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        Display();
    }
}
