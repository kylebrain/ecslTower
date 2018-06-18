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
    private Leaderboard leaderboard;
    private UnityEngine.UI.InputField inputField;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        winAudio = audioSources[0];
        loseAudio = audioSources[1];

        screen = transform.Find("EndScreen").gameObject;
        leaderboard = screen.transform.Find("Highscores").GetComponent<Leaderboard>();
        inputField = screen.transform.Find("Name").GetComponent<UnityEngine.UI.InputField>();
    }

    public void EndGame(bool won, int score = 0)
    {
        if (won)
        {
            screen.transform.Find("Score").GetComponent<Text>().text = "Score: " + score;
            winAudio.Play();
            LevelUnlocking.AddToUnlocked(LevelLookup.levelNumber + 1);
            StartCoroutine(DisplayEnd(won, winAudio.clip.length)); //plays the sound then waits to show screen
        } else
        {
            screen.transform.Find("Score").GetComponent<Text>().text = "You were destroyed!";
            StartCoroutine(DisplayEnd(won, loseAudio.clip.length)); //waits for the final explosion before showing the screen
        }
        
    }

	// Update is called once per frame
	void Update () {
        if (screen.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                leaderboard.AddNewHighscore(inputField.text, Score.score);
            }
        }
	}

    private void Display(bool won)
    {
        previousAudioVolume = AudioListener.volume;
        AudioListener.volume = 0; //or find another way to turn off the game sfx maybe with an audio mixer?
        transform.parent.GetComponent<Canvas>().sortingOrder = 1; //this should overlay over the router UI
        screen.SetActive(true);
        leaderboard.DownloadHighscores();
        if (won)
        {
            inputField.gameObject.SetActive(true);
        }
    }

    IEnumerator DisplayEnd(bool won, float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        Display(won);
    }

    public void ReturnToLevelSelect()
    {
        AudioListener.volume = previousAudioVolume;
        SceneManager.LoadScene("LevelSelect");
    }

}
