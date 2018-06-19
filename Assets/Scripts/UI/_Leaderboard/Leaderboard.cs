using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{

    const string webURL = "http://dreamlo.com/lb/";
    public Highscore[] highscoresList;
    private LeaderboardDisplay display;
    private static Leaderboard instance;
    public static Highscore fetchedHighscore = null;

    private void Awake()
    {
        instance = this;
        display = GetComponent<LeaderboardDisplay>();
        fetchedHighscore = null;
    }

    public static void AddNewHighscore(string username, int score)
    {
        instance.StartCoroutine(instance.UploadNewHighscore(username, score));
    }

    IEnumerator UploadNewHighscore(string username, int score, string privateCode = null)
    {
        if(privateCode == null)
        {
            privateCode = LevelLookup.privateLeaderboardCode;
        }
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
            DownloadHighscores();
        }
        else
        {
            print("Error uploading: " + www.error);
        }
    }

    public static void DownloadHighscores(string publicCode = null)
    {
        if(publicCode == null)
        {
            publicCode = LevelLookup.publicLeaderboardCode;
        }
        if (string.IsNullOrEmpty(publicCode))
        {
            return;
        }
        instance.StartCoroutine(instance.DownloadHighscoresFromDatabase(publicCode));
    }

    IEnumerator DownloadHighscoresFromDatabase(string publicCode)
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            if (display != null)
            {
                display.OnHighscoresDownloaded(highscoresList);
            }
            if(highscoresList.Length > 0)
            {
                fetchedHighscore = highscoresList[0];
            } else
            {
                fetchedHighscore = Highscore.nullValue;
            }
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }

    void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
        }
    }

}

public class Highscore
{
    public string username;
    public int score;

    public static Highscore nullValue
    {
        get
        {
            return new Highscore("DEFAULT", -1);
        }
    }

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }

    public override bool Equals(object obj)
    {
        if(obj.GetType() != typeof(Highscore))
        {
            return false;
        }
        Highscore highscore = (Highscore)obj;
        return username == highscore.username && score == highscore.score;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
