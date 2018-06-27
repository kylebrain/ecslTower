using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public StarScript starScript;
    private float previousAudioVolume;
    private AudioSource loseAudio; //make sure to change this sound to reflect the malicious agent sound
    private GameObject screen;
    private UnityEngine.UI.InputField inputField;
    private bool highscoreSubmitted = false;
    private int finalScore = 0;
    private bool endGame = false;

    private void Awake()
    {
        loseAudio = GetComponent<AudioSource>();

        screen = transform.Find("EndScreen").gameObject;
        inputField = screen.transform.Find("Name").GetComponent<UnityEngine.UI.InputField>();
        endGame = false;
    }

    public void EndGame(bool won, int score = 0)
    {
        if (endGame)
        {
            return;
        }
        endGame = true;
        finalScore = score;
        if (won)
        {
            screen.transform.Find("Score").GetComponent<Text>().text = "Score: " + score;
            AudioManager.Mute("Theme");
            AudioManager.Play("Victory");
            LevelUnlocking.AddToUnlocked(LevelLookup.levelNumber + 1);
            StartCoroutine(DisplayEnd(won, AudioManager.GetLength("Victory"), score)); //plays the sound then waits to show screen
        }
        else
        {
            screen.transform.Find("Score").GetComponent<Text>().text = "You were destroyed!";
            StartCoroutine(DisplayEnd(won, loseAudio.clip.length, score)); //waits for the final explosion before showing the screen
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            EndGame(true, Score.Money);
        }
        if (screen.activeSelf && Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(inputField.text) && !highscoreSubmitted && inputField.interactable)
        {
            Leaderboard.AddMapHighscore(inputField.text, finalScore, LevelLookup.levelNumber);
            PlayerPrefs.SetString("highscoreUsername", inputField.text);
            highscoreSubmitted = true;
            inputField.interactable = false;
        }
    }

    private void Display(bool won, int score)
    {
        previousAudioVolume = AudioListener.volume;
        AudioListener.volume = 0; //or find another way to turn off the game sfx maybe with an audio mixer?
        transform.parent.GetComponent<Canvas>().sortingOrder = 1; //this should overlay over the router UI
        screen.SetActive(true);
        if(won)
        {
            starScript.Display(score);
        }
        Leaderboard.DownloadHighscores();
        
    }

    IEnumerator DisplayEnd(bool won, float afterSeconds, int score)
    {
        yield return new WaitForSeconds(afterSeconds);
        Display(won, score);
        if (won)
        {
            inputField.gameObject.SetActive(true);
            inputField.text = PlayerPrefs.GetString("highscoreUsername", null);
        }
    }

    public void ReturnToLevelSelect()
    {
        AudioListener.volume = previousAudioVolume;
        AudioManager.Mute("Theme", false);
        SceneLoader.LoadScene("LevelSelect");
    }

}
