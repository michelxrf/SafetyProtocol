using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Presents the end of game report, showing player stats for the level
/// </summary>
public class GameEndScreen : MonoBehaviour
{
    UIDocument uiDocument;

    VisualElement header;
    Label playerScoreEntry = null;

    Label scoreLabel;
    Label timeLabel;
    Label accidentsLabel;
    Label hazzardsLabel;

    [Header("Audio")]
    [SerializeField] AudioClip showGameEndSFX;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Button>("ContinueButton").clicked += OnBackToMenuClicked;
        uiDocument.rootVisualElement.Q<Button>("ContinueButton").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));

        scoreLabel = uiDocument.rootVisualElement.Q<Label>("Score");
        timeLabel = uiDocument.rootVisualElement.Q<Label>("Time");
        accidentsLabel = uiDocument.rootVisualElement.Q<Label>("Accidents");
        hazzardsLabel = uiDocument.rootVisualElement.Q<Label>("Hazzards");

        header = uiDocument.rootVisualElement.Q<VisualElement>("Header");
        header.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        LeaderboardManager.Instance.OnLeaderboardReceived += PopulateLeaderboard;
        LeaderboardManager.Instance.OnScoreSubmitted += GetScores;
    }

    /// <summary>
    /// Loads the data that will be shown
    /// </summary>
    /// <param name="score">Player score</param>
    /// <param name="time">Time spent on level</param>
    /// <param name="accidentsSolved">solved accidents</param>
    /// <param name="totalAccidents">total accidents on the level</param>
    /// <param name="hazzardsSolved">solved hazzards</param>
    /// <param name="totalHazzards">total hazzards on level</param>
    public void Show(int score, float time, int accidentsSolved, int totalAccidents, int hazzardsSolved, int totalHazzards)
    {
        // prevents click handling bug as it was destroied during scene change
        ClickHandler.Instance.canClick = false;
        AudioManager.Instance.PlaySFX(showGameEndSFX, transform);

        scoreLabel.text = $"SUA PONTUAÇÃO: {score.ToString()}";
        timeLabel.text = $"TEMPO TOTAL: {time.ToString($"#0.0")} Segundos";
        accidentsLabel.text = $"ACIDENTES PREVENIDOS: {accidentsSolved.ToString()}/{totalAccidents.ToString()}";
        hazzardsLabel.text = $"RISCOS ELIMINADOS: {hazzardsSolved.ToString()}/{totalHazzards.ToString()}";
    }

    /// <summary>
    /// Calls Playfab for scores
    /// </summary>
    private void GetScores()
    {
        LeaderboardManager.Instance.StartCoroutine(LeaderboardManager.Instance.GetScoresAsync());
    }

    /// <summary>
    /// Clears all entries from the leaderboard.
    /// </summary>
    private void ClearLeaderboard()
    {
        VisualElement leaderboardParent = uiDocument.rootVisualElement.Q<VisualElement>("Leaderboard");

        VisualElement[] allEntries = leaderboardParent.Children().ToArray();
        foreach (VisualElement entry in allEntries)
        {
            if (entry.name == "PlayerEntry")
                leaderboardParent.Remove(entry);
        }

        playerScoreEntry = null;
    }

    /// <summary>
    /// Instantiate lines in the leaderboard, each line is a rank/player/score entry
    /// </summary>
    private void PopulateLeaderboard(GetLeaderboardResult playfabData)
    {
        ClearLeaderboard();

        VisualElement leaderboardParent = uiDocument.rootVisualElement.Q<VisualElement>("Leaderboard");

        // show header
        header.style.display = DisplayStyle.Flex;

        // Instantiate each score entry
        foreach (var entry in playfabData.Leaderboard)
        {
            VisualElement entryParent = new VisualElement();
            entryParent.name = "PlayerEntry";

            Label rank = new Label();
            rank.text = (entry.Position + 1).ToString() + "º";
            rank.AddToClassList("RankColumm");
            entryParent.Add(rank);

            Label playerName = new Label();
            playerName.text = entry.DisplayName;
            playerName.AddToClassList("NameColumm");
            entryParent.Add(playerName);

            Label score = new Label();
            score.text = entry.StatValue.ToString();
            score.AddToClassList("ScoreColumm");
            entryParent.Add(score);

            // highlights the entry for this player's score
            if (entry.PlayFabId == LeaderboardManager.Instance.playerID)
            {
                entryParent.AddToClassList("PlayerScoreEntry");
                playerScoreEntry = playerName;
            }
            else
            {
                entryParent.AddToClassList("ScoreEntry");
            }

            leaderboardParent.Add(entryParent);

            uiDocument.rootVisualElement.Q<Label>("Notice").style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Goes back to menus
    /// </summary>
    private void OnBackToMenuClicked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Clears callbacks on Leaderboard Manager Singleton
    /// </summary>
    private void OnDestroy()
    {
        LeaderboardManager.Instance.OnLeaderboardReceived = null;
        LeaderboardManager.Instance.OnEmptyLeadearboardReceived = null;
        LeaderboardManager.Instance.OnScoreSubmitted = null;
    }
}
