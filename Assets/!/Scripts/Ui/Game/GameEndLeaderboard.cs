using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


/// <summary>
/// Displays the leaderboard at the end of the level
/// </summary>
public class GameEndLeaderboard : MonoBehaviour
{
    UIDocument uiDocument;
    VisualElement header;
    Label playerScoreEntry = null;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Button>("ContinueButton").clicked += OnBackToMenuClicked;

        header = uiDocument.rootVisualElement.Q<VisualElement>("Header");
        header.style.display = DisplayStyle.None;
    }

    private void GetScores()
    {
        LeaderboardManager.Instance.GetTop10Scores();
    }

    private void Start()
    {
        LeaderboardManager.Instance.OnLeaderboardReceived += PopulateLeaderboard;
        LeaderboardManager.Instance.OnScoreSubmitted += GetScores;
    }

    private void PopulateLeaderboard(GetLeaderboardResult playfabData)
    {
        Debug.Log("Should update leadeboard...");

        VisualElement leaderboardParent = uiDocument.rootVisualElement.Q<VisualElement>("Leaderboard");

        // show header
        header.style.display = DisplayStyle.Flex;

        // Instantiate each score entry
        foreach (var entry in playfabData.Leaderboard)
        {
            VisualElement entryParent = new VisualElement();
            entryParent.name = "PlayerEntry";

            Label rank = new Label();
            rank.text = (entry.Position + 1).ToString();
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

    private void OnBackToMenuClicked()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        LeaderboardManager.Instance.OnLeaderboardReceived -= PopulateLeaderboard;
        LeaderboardManager.Instance.OnEmptyLeadearboardReceived -= ClearLeaderboard;
    }
}
