using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

/// <summary>
/// Controls access and conection to Playfab backend, used for the game's leaderboard
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    //TODO: conect to some ui element that displays the scores
    //[HideInInspector] public LeaderboardUi leaderboardUi;

    // Inits all player and leaderboard info as an unitialized state
    [HideInInspector] public string playerID;
    private string playerName = String.Empty;
    private int playerScore = -1;
    private string currentLeaderboardName = String.Empty;

    [Header("Connection Settings")]
    [SerializeField] float forcedRetryInterval = 10f; // After an failed attempt to conect, tries again after this amount of seconds
    [SerializeField] float retryInterval = 2f; // wait this time before a new attempt to connect, to avoid abusing the backend

    // reconection retry limits
    [SerializeField] int maxAuthRetries = 3;
    [SerializeField] int maxNameChangeRetries = 3;
    [SerializeField] int maxSubmitScoreRetries = 3;
    [SerializeField] int maxGetLeaderboardRetries = 3;
    [SerializeField] int maxPlayerScoreRetries = 3;

    // retry conection attemp counters
    private int retryAuthCount = 0;
    private int retryNameChangeCount = 0;
    private int retryGetLeaderboardCount = 0;
    private int retrySubmitScoreCount = 0;
    private int retryPlayerScoreCount = 0;

    // flags for conection state control
    private bool loggedIn = false;
    private bool nameSubmitted = false;
    private bool scoreSubmitted = false;
    private bool retrieveLeaderboardOnAuth = false;
    private bool retrieveLeaderboardOnSubmit = false;
    private bool waitingAutoReconnect = false;
    private bool retrievePlayerScoreOnAuth = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // prevents a second Singleton from being created
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            AuthAsGuest();
        }
    }

    /// <summary>
    /// Does the user auth and login, required to identify the player in the backend, it uses device id as unique.
    /// </summary>
    private void AuthAsGuest()
    {
        var request = new LoginWithCustomIDRequest()
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnAuthSuccess, OnAuthError);
        Debug.Log("Trying to sign player into PlayFab");
    }

    /// <summary>
    /// Called when and auth request was successful.
    /// </summary>
    /// <param name="result">Contains all user login data like id.</param>
    private void OnAuthSuccess(LoginResult result)
    {
        // Updates conection flags and counters
        playerID = result.PlayFabId;
        loggedIn = true;
        retryAuthCount = 0;

        Debug.Log("Succesfully signed in");

        // Calls to change player name after auth, used in reconections
        if (!nameSubmitted && playerName.Length > 0)
        {
            ChangePlayerName(playerName);
        }

        // Calls to submit score after a successful auth, used in reconection attempts
        if (!scoreSubmitted && playerScore >= 0)
        {
            SubmitCurrentScore(playerScore, currentLeaderboardName);
        }

        // Calls to retrieve the leaderboard after a succesful auth, used in reconnect attempts
        if (retrieveLeaderboardOnAuth)
        {
            retrieveLeaderboardOnAuth = false;
            GetScoresAsync(currentLeaderboardName);
        }

        // Calls to retrieve the player's score after a sucessful reconect attempt
        if (retrievePlayerScoreOnAuth)
        {
            retrievePlayerScoreOnAuth = false;
            GetPlayerScoreAsync(currentLeaderboardName);
        }
    }

    /// <summary>
    /// Called when failed to log the user in to the backend.
    /// </summary>
    /// <param name="error">Error code info.</param>
    private void OnAuthError(PlayFabError error)
    {
        loggedIn = false;
        
        // prints error to console
        Debug.Log(error.GenerateErrorReport());

        // Tries to reconnect
        if (retryAuthCount < maxAuthRetries)
        {
            retryAuthCount++;
            float retryTime = retryAuthCount * retryInterval;
            Debug.Log($"Conection failed {retryAuthCount}/{maxAuthRetries}, trying again in {retryTime}s");
            StartCoroutine(RetryAuthConection(retryTime));
        }
        else
        {
            // failed to connect to Playfab

            Debug.Log("Failed connecting to Playfab");
            if (retrieveLeaderboardOnAuth/* && leaderboardUi != null*/)
            {
                NoConnection();
            }
        }
    }

    /// <summary>
    /// Calls an attempt to reconnect to the backend after a time interval. This delay is necessary to minimize backend calls.
    /// </summary>
    /// <param name="delay">The amount of time it'll wait before making the log in attempt.</param>
    /// <returns></returns>
    private IEnumerator RetryAuthConection(float delay)
    {
        yield return new WaitForSeconds(delay);
        AuthAsGuest();
    }

    /// <summary>
    /// Changes the user name in the backend.
    /// </summary>
    /// <param name="newPlayerName">The new name.</param>
    public void ChangePlayerName(string newPlayerName)
    {
        Debug.Log($"uploading new player name: {newPlayerName}");
        playerName = newPlayerName;

        // forces a new log in, used in case of lost conections
        if (!loggedIn)
        {
            AuthAsGuest();
            return;
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = newPlayerName,
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnNameChangeSuccess, OnNameChangeError);
    }

    /// <summary>
    /// Called when the backend responds with a successful name change.
    /// </summary>
    /// <param name="result">Backend response.</param>
    private void OnNameChangeSuccess(UpdateUserTitleDisplayNameResult result)
    {
        retryNameChangeCount = 0;
        nameSubmitted = true;
        Debug.Log($"player name is now: {result.DisplayName}");
    }

    /// <summary>
    /// Called when name changed failed.
    /// </summary>
    /// <param name="error">Error information.</param>
    private void OnNameChangeError(PlayFabError error)
    {
        // prints the error code to console
        Debug.Log(error.GenerateErrorReport());
        nameSubmitted = false;

        if (loggedIn)
        {
            if (retryNameChangeCount < maxNameChangeRetries)
            {
                // calls for a retry
                retryNameChangeCount++;
                float retryTime = retryNameChangeCount * retryInterval;
                Debug.Log($"Namechange failed {retryNameChangeCount}/{maxNameChangeRetries}, trying again in {retryTime}s");
                StartCoroutine(RetryNameChangeConection(retryTime));
            }
            else
            {
                // max reconnect attempts reached, gives up
                Debug.Log("Failed connecting to Playfab");
            }
        }
        else
        {
            // tries to log in once again, used for reconection attempts
            AuthAsGuest();
        }
    }

    /// <summary>
    /// Tries to change the username once again after a time interval.
    /// </summary>
    /// <param name="delay">The amount of time it'll wait before making the attempt</param>
    /// <returns></returns>
    private IEnumerator RetryNameChangeConection(float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangePlayerName(playerName);
    }


    /// <summary>
    /// Sends the current player score to the backend.
    /// </summary>
    /// <param name="score">The score to send.</param>
    /// <param name="leaderboardName">The leaderboard it will register the score to.</param>
    public void SubmitCurrentScore(int score, string leaderboardName)
    {
        playerScore = score;
        currentLeaderboardName = leaderboardName;

        // retries to login, used when connection was lost
        if (!loggedIn)
        {
            AuthAsGuest();
            return;
        }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        Debug.Log($"Sending player score to Playfab: {playerScore}");
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSubmitScoreSuccess, OnSubmitScoreError);

    }

    /// <summary>
    /// Called when the backend successfully received the new score.
    /// </summary>
    /// <param name="result">Backend response</param>
    void OnSubmitScoreSuccess(UpdatePlayerStatisticsResult result)
    {
        scoreSubmitted = true;

        if (retrieveLeaderboardOnSubmit)
        {
            GetTop10Scores(currentLeaderboardName);
        }

        Debug.Log("score succesfully submitted to Playfab");
    }

    /// <summary>
    /// Called when an error has ocorred when sending the scores. Will try to resend.
    /// </summary>
    /// <param name="error">Error code.</param>
    private void OnSubmitScoreError(PlayFabError error)
    {
        // print the error to console
        Debug.Log(error.GenerateErrorReport());
        scoreSubmitted = false;

        if (loggedIn)
        {
            if (retrySubmitScoreCount < maxSubmitScoreRetries)
            {
                // tries to resend

                retrySubmitScoreCount++;
                float retryTime = retrySubmitScoreCount * retryInterval;
                Debug.Log($"Score submition failed {retrySubmitScoreCount}/{maxSubmitScoreRetries}, trying again in {retryTime}s");
                StartCoroutine(RetrySubmitScoreConection(retryTime));
            }
            else
            {
                // too many failed retries, no connection

                Debug.Log("Failed sending player score to Playfab");
                if (retrieveLeaderboardOnSubmit)
                {
                    // Sets a "no connection" notice
                    NoConnection();
                }
            }
        }
        else
        {
            // tries to login
            AuthAsGuest();
        }
    }

    /// <summary>
    /// Attempts to resend scores after a delay, used after a failed attempt.
    /// </summary>
    /// <param name="delay">Wait this amount of seconds to try.</param>
    /// <returns></returns>
    private IEnumerator RetrySubmitScoreConection(float delay)
    {
        yield return new WaitForSeconds(delay);
        SubmitCurrentScore(playerScore, currentLeaderboardName);
    }


    /// <summary>
    /// Calls the backend for the best 10 scores of the leaderboard.
    /// </summary>
    /// <param name="leaderboardName">The leaderboard it'll pick from.</param>
    public void GetTop10Scores(string leaderboardName)
    {
        currentLeaderboardName = leaderboardName;

        // retry to log in, if conection was lost at some point
        if (!loggedIn)
        {
            retrieveLeaderboardOnAuth = true;
            AuthAsGuest();
            return;
        }

        Debug.Log("called backend for the top 10 scores");

        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardError);
    }

    /// <summary>
    /// Called once the top 10 scores were received from the backend.
    /// </summary>
    /// <param name="result">The backend response data.</param>
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        // TODO: deals with empity leaderboards

        Debug.Log("Leaderboard scores received");

        // TODO: tells the UI element to fill it's scores
        /*
        if (leaderboardUi != null)
        {
            leaderboardUi.FillScores(result);
        }
        else
        {
            Debug.LogError("LeaderboardManager has no UI ref");
        }
        */
    }

    /// <summary>
    /// Couldn't not get the top 10 scores. Try again.
    /// </summary>
    /// <param name="error">Error info.</param>
    private void OnGetLeaderboardError(PlayFabError error)
    {
        // Prints the error info to console
        Debug.Log(error.GenerateErrorReport());

        if (loggedIn)
        {
            if (retryGetLeaderboardCount < maxGetLeaderboardRetries)
            {
                // Tries again

                retryGetLeaderboardCount++;
                float retryTime = retryGetLeaderboardCount * retryInterval;
                Debug.Log($"Leaderboard retrieval failed {retryGetLeaderboardCount}/{maxGetLeaderboardRetries}, trying again in {retryTime}s");
                StartCoroutine(RetryGetLeaderboardConection(retryTime));
            }
            else
            {
                // Too many faild attempts, no connection

                NoConnection();
                Debug.Log("Failed retrieving leaderboard from Playfab");
            }
        }
        else
        {
            // Will try again after attempting a new log in

            retrieveLeaderboardOnAuth = true;
            AuthAsGuest();
        }
    }

    /// <summary>
    /// Try to get the top 10 again after a time delay.
    /// </summary>
    /// <param name="delay">The time it'll wait before a new attempt.</param>
    /// <returns></returns>
    private IEnumerator RetryGetLeaderboardConection(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetTop10Scores(currentLeaderboardName);
    }


    /// <summary>
    /// A delayed score call, verify if it still fits the use. TODO: This might not make sense in the current project.
    /// </summary>
    /// <param name="leaderboardName">Leaderboard to send the scores to.</param>
    /// <returns></returns>
    public IEnumerator GetScoresAsync(string leaderboardName)
    {
        yield return new WaitForSeconds(2);

        if (!scoreSubmitted)
        {
            // queue a score submition before the retrieval
            retrieveLeaderboardOnAuth = true;
            AuthAsGuest();
        }
        else
        {
            GetTop10Scores(leaderboardName);
        }
    }

    /// <summary>
    /// A delayed score call, verify if it still fits the use. TODO: This might not make sense in the current project.
    /// </summary>
    /// <param name="leaderboardName"></param>
    /// <returns></returns>
    public IEnumerator GetPlayerScoreAsync(string leaderboardName)
    {
        yield return new WaitForSeconds(2);

        if (!scoreSubmitted)
        {
            // queue a score submition before the retrieval
            retrievePlayerScoreOnAuth = true;
            AuthAsGuest();
        }
        else
        {
            GetPlayerScore(leaderboardName);
        }
    }

    /// <summary>
    /// Calls the backend to retrieve only the player's last score submited.
    /// </summary>
    /// <param name="leaderboardName">The leaderboard to get the score from.</param>
    public void GetPlayerScore(string leaderboardName)
    {
        currentLeaderboardName = leaderboardName;

        // forces a login retry, in case of lost connections
        if (!loggedIn)
        {
            retrievePlayerScoreOnAuth = true;
            AuthAsGuest();
            return;
        }

        Debug.Log("called backend for the player score");

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetPlayerScoreSuccess, OnGetPlayerScoreError);
    }

    /// <summary>
    /// Player's last score received from the backend after a call.
    /// </summary>
    /// <param name="result">The backend response.</param>
    private void OnGetPlayerScoreSuccess(GetLeaderboardAroundPlayerResult result)
    {
        Debug.Log("best player score received");
        //TODO: connect to UI

        /*
        if (leaderboardUi != null)
        {
            leaderboardUi.ShowBestPlayerScore(result.Leaderboard[0].StatValue);
        }
        else
        {
            Debug.LogError("LeaderboardManager has no UI ref");
        }
        */
    }

    /// <summary>
    /// Error return on trying to retrieve player's score.
    /// </summary>
    /// <param name="error">Error data.</param>
    private void OnGetPlayerScoreError(PlayFabError error)
    {
        // prints the error data to console
        Debug.Log(error.GenerateErrorReport());

        if (loggedIn)
        {
            if (retryPlayerScoreCount < maxPlayerScoreRetries)
            {
                // retries sending the player score

                retryPlayerScoreCount++;
                float retryTime = retryPlayerScoreCount * retryInterval;
                Debug.Log($"Player score retrieval failed {retryPlayerScoreCount}/{maxPlayerScoreRetries}, trying again in {retryTime}s");
                StartCoroutine(RetryGetPlayerScoreConection(retryTime));
            }
            else
            {
                // Gives up after too many calls

                NoConnection();
                Debug.Log("Failed retrieving player score from Playfab");
            }
        }
        else
        {
            // queue to try again after retrying to login

            retrievePlayerScoreOnAuth = true;
            AuthAsGuest();
        }
    }

    /// <summary>
    /// A delayed retry attempt to retrieve player's score.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator RetryGetPlayerScoreConection(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetPlayerScore(currentLeaderboardName);
    }


    /// <summary>
    /// Notifies that no conection was stablished
    /// </summary>
    private void NoConnection()
    {
        // TOOD: Tells an UI element that there's no connection

        if (waitingAutoReconnect)
            return;

        //leaderboardUi.NoConnection();

        retrieveLeaderboardOnSubmit = true;
        retrieveLeaderboardOnAuth = true;
        StartCoroutine(AutoReconnect());
    }

    /// <summary>
    /// Attempts to reconect to the backend after a time interval after no connection was succesfully stablished.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoReconnect()
    {
        // TODO: Notifie UI

        waitingAutoReconnect = true;
        int countdown = (int)forcedRetryInterval;
        while (countdown > 0)
        {
            yield return new WaitForSeconds(1f);

            countdown--;
            /*
            if (leaderboardUi != null)
            {
                leaderboardUi.WaitingReconnect(countdown);
            }
            */
        }

        GetTop10Scores(currentLeaderboardName);
        waitingAutoReconnect = false;
        //leaderboardUi.Connecting();
    }

    /// <summary>
    /// Clears all connection states and data. Is it still useful?
    /// </summary>
    public void ResetLeaderboard()
    {
        StopAllCoroutines();
        playerScore = -1;
        currentLeaderboardName = String.Empty;
        retryAuthCount = 0;
        retryNameChangeCount = 0;
        retryGetLeaderboardCount = 0;
        retrySubmitScoreCount = 0;
        scoreSubmitted = false;
        retrieveLeaderboardOnAuth = false;
        retrieveLeaderboardOnSubmit = false;

    }
    
}