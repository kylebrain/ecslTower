using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    const int scoresPerBoard = 10;
    const int maxScores = 1000;
    const int numberOfBoards = maxScores / scoresPerBoard;

    const string webURL = "https://www.dreamlo.com/lb/";
    readonly string privateCode = LeaderBoardKeys.PrivateKey;
    readonly string publicCode = LeaderBoardKeys.PublicKey;

    //create the class with the keys from the google drive
    //must be placed in the UI/_Leaderboard folder
        //see gitignore for location

    //public Highscore[] highscoresList;
    private LeaderboardDisplay display;
    private static Leaderboard instance;
    //public static Highscore fetchedHighscore = null;
    public static List<Highscore>[] mapHighscores = new List<Highscore>[numberOfBoards];
    public static bool Downloaded = false;

    private void Awake()
    {
        instance = this;
        display = GetComponent<LeaderboardDisplay>();
    }

    public static void AddMapHighscore(string name, int score, int mapNumber)
    {
        AddNewHighscore(mapNumber.ToString("000") + name, score, mapNumber);
    }

    private static void AddNewHighscore(string username, int score, int mapNumber)
    {
        if (mapNumber <= 0 || mapNumber > 100)
        {
            Debug.LogError("Map number must be greater than 0 and less than or equal to 100!");
            return;
        }
        instance.StartCoroutine(instance.UploadNewHighscore(username, score, mapNumber));
    }

    IEnumerator UploadNewHighscore(string username, int score, int mapNumber)
    {
        DownloadHighscores();
        if (IsHighscoreAcceptable(score, mapNumber))
        {
            if (!IsReplacing(new Highscore(username, score, mapNumber)))
            {
                RemoveLowestHighScore(score, mapNumber);
            }
            WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score + "/" + mapNumber);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                print("Upload Successful: " + username);
                DownloadHighscores();
            }
            else
            {
                print("Error uploading: " + www.error);
            }
        }
        yield return null;
    }

    public static bool IsHighscoreAcceptable(int score, int mapNumber)
    {
        List<Highscore> highscoreList = mapHighscores[mapNumber - 1];
        if (highscoreList == null || highscoreList.Count < scoresPerBoard)
        {
            return true;
        }
        Highscore delHighscore = highscoreList[highscoreList.Count - 1];
        if (score > delHighscore.score)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsReplacing(Highscore highscore)
    {
        List<Highscore> highscoreList = mapHighscores[highscore.mapNumber - 1];
        bool ret = true;
        if (highscoreList != null)
        {
            Highscore exisitingScore = mapHighscores[highscore.mapNumber - 1].Find(x => x.username == highscore.cleanUsername);
            if (exisitingScore != null)
            {
                ret = highscore.score > exisitingScore.score;
            }
        }
        return ret;
    }

    private void RemoveLowestHighScore(int score, int mapNumber)
    {
        if (!IsHighscoreAcceptable(score, mapNumber))
        {
            return;
        }
        List<Highscore> highscoreList = mapHighscores[mapNumber - 1];
        if (highscoreList.Count < scoresPerBoard)
        {
            return;
        }
        Highscore delHighscore = highscoreList[highscoreList.Count - 1];
        StartCoroutine(DeleteHighScore(delHighscore.mapNumber.ToString("000") + delHighscore.username));
    }

    IEnumerator DeleteHighScore(string username)
    {
        WWW www = new WWW(webURL + privateCode + "/delete/" + WWW.EscapeURL(username));
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            print("Delete Success: " + username);
        }
        else
        {
            print(www.error);
        }
    }

    public static void DownloadHighscores()
    {
        instance.StartCoroutine(instance.DownloadHighscoresFromDatabase());
    }

    IEnumerator DownloadHighscoresFromDatabase()
    {
        Downloaded = false;
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            if (display != null)
            {
                display.OnHighscoresDownloaded(mapHighscores[LevelLookup.levelNumber - 1]);
            }
            /*
            if(highscoresList.Length > 0)
            {
                fetchedHighscore = highscoresList[0];
            } else
            {
                fetchedHighscore = Highscore.nullValue;
            }
            */
            Downloaded = true;
        }
        else
        {
            Debug.LogError("Error Downloading: " + www.error);
        }
    }

    void FormatHighscores(string textStream)
    {
        mapHighscores = new List<Highscore>[numberOfBoards];

        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        //highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            //Debug.Log(entries[i]);
            Highscore newHighscore = new Highscore(entries[i]);
            //Debug.Log(newHighscore.username + "|" + newHighscore.score + "|" + newHighscore.mapNumber);
            if (newHighscore.mapNumber > 0)
            {
                //highscoresList[i] = newHighscore;
                if (mapHighscores[newHighscore.mapNumber - 1] == null)
                {
                    mapHighscores[newHighscore.mapNumber - 1] = new List<Highscore>();
                }
                mapHighscores[newHighscore.mapNumber - 1].Add(newHighscore);
            }
            else
            {
                Debug.LogError("Map number must be greater than 0");
            }
        }
    }

}

public class Highscore
{
    public string username;
    public int score;
    public int mapNumber;

    public static Highscore nullValue
    {
        get
        {
            return new Highscore("DEFAULT", -1, -1);
        }
    }

    public string cleanUsername
    {
        get
        {
            return Regex.Replace(username, @"[\d-]", string.Empty);
        }
    }

    public Highscore(string _username, int _score, int _mapNumber)
    {
        username = _username;
        score = _score;
        mapNumber = _mapNumber;
    }

    public Highscore(string dataString)
    {
        string[] entryInfo = dataString.Split(new char[] { '|' });
        username = Regex.Replace(entryInfo[0], @"[\d-]", string.Empty);
        score = int.Parse(entryInfo[1]);
        mapNumber = int.Parse(entryInfo[2]);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(Highscore))
        {
            return false;
        }
        Highscore highscore = (Highscore)obj;
        return username == highscore.username && score == highscore.score && mapNumber == highscore.mapNumber;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
