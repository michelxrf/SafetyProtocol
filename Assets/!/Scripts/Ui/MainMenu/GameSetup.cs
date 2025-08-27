using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Controls the screen that will be used to set up the game.
/// </summary>
public class GameSetup : MonoBehaviour
{
    UIDocument uiDocument;
    [SerializeField] TitleScreen tittleScreen;

    VisualElement header;

    TextField leaderboardNameInput;
    TextField usernameInput;

    RadioButtonGroup difficultySetting;
    RadioButtonGroup gameMap;

    Button startGameButton;

    Label playerScoreEntry = null;

    [Header("Profanity Blocklist")]
    [SerializeField] private TextAsset[] profaneListFiles;
    private List<string> profanityList = new List<string>();

    private void Awake()
    {
        // set up game setup screen
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;

        // init "voltar" button
        uiDocument.rootVisualElement.Q<Button>("Back").clicked += ShowTitle;

        // init "jogar" button
        startGameButton = uiDocument.rootVisualElement.Q<Button>("Play");
        startGameButton.SetEnabled(false);
        startGameButton.clicked += StartGame;

        // init username inputfield
        usernameInput = uiDocument.rootVisualElement.Q<TextField>("UserName");
        usernameInput.isDelayed = true;
        usernameInput.RegisterValueChangedCallback(evt => { UsernameChanged(usernameInput.value); usernameInput.Blur(); });
        usernameInput.RegisterCallback<FocusOutEvent>(evt => { VerifyAndAllowGameStart(); });
        usernameInput.RegisterCallback<FocusInEvent>(evt => { LockGameStartOnEdit(); });
        usernameInput.RegisterCallback<KeyDownEvent>(FilterInputChars, TrickleDown.TrickleDown);
        usernameInput.maxLength = 25;

        // init "nome da turma" input field
        leaderboardNameInput = uiDocument.rootVisualElement.Q<TextField>("LeaderboardName");
        leaderboardNameInput.isDelayed = true;
        leaderboardNameInput.RegisterValueChangedCallback(evt => { ClassNameChanged(leaderboardNameInput.value); leaderboardNameInput.Blur(); });
        leaderboardNameInput.RegisterCallback<FocusInEvent>(evt => { LockGameStartOnEdit(); });
        leaderboardNameInput.RegisterCallback<FocusOutEvent>(evt => VerifyAndAllowGameStart());
        leaderboardNameInput.RegisterCallback<KeyDownEvent>(FilterInputChars, TrickleDown.TrickleDown);
        leaderboardNameInput.maxLength = 25;

        // init level select
        gameMap = uiDocument.rootVisualElement.Q<RadioButtonGroup>("Map");
        gameMap.RegisterValueChangedCallback(evt => { ClassNameChanged(leaderboardNameInput.value); });

        // init difficulty select
        difficultySetting = uiDocument.rootVisualElement.Q<RadioButtonGroup>("Difficulty");
        difficultySetting.RegisterValueChangedCallback(evt => { ClassNameChanged(leaderboardNameInput.value); });

        header = uiDocument.rootVisualElement.Q<VisualElement>("Header");
        header.style.display = DisplayStyle.None;

        // loads profanity blocklist
        foreach(TextAsset profaneFile in profaneListFiles)
        {
            profanityList.AddRange(profaneFile.text.Split("\n"));
        }
        if(profanityList.Count > 0)
            EnhanceProfanityByVariety();
    }

    private void Start()
    {
        // loads the screen with the held data of the session
        if (SettingsKeeper.Instance.classRoomName != null)
        {
            leaderboardNameInput.SetValueWithoutNotify(SettingsKeeper.Instance.classRoomName);
            
            foreach (string option in difficultySetting.choices.ToList())
            {
                if (option == SettingsKeeper.Instance.dificultyLevel)
                {
                    difficultySetting.SetValueWithoutNotify(difficultySetting.choices.ToList().IndexOf(option));
                    break;
                }
            }
            foreach (string option in gameMap.choices.ToList())
            {
                if (option == SettingsKeeper.Instance.gameMap)
                {
                    gameMap.SetValueWithoutNotify(gameMap.choices.ToList().IndexOf(option));
                    break;
                }
            }

            LeaderboardManager.Instance.GetTop10Scores();
            LeaderboardManager.Instance.GetPlayerScore();
        }

        if(LeaderboardManager.Instance.playerName != null)
        {
            usernameInput.SetValueWithoutNotify(LeaderboardManager.Instance.playerName);
        }

        VerifyAndAllowGameStart();

        LeaderboardManager.Instance.OnSignIn += NameReceived;
        LeaderboardManager.Instance.OnLeaderboardReceived += PopulateLeaderboard;
        LeaderboardManager.Instance.OnEmptyLeadearboardReceived += ShowEmptyLeaderboard;
    }

    /// <summary>
    /// Hides this screen and shows the title screen
    /// </summary>
    private void ShowTitle()
    {
        Hide();
        tittleScreen.Show();
    }

    /// <summary>
    /// Auto fill player name
    /// </summary>
    private void NameReceived()
    {
        usernameInput.SetValueWithoutNotify(LeaderboardManager.Instance.playerName);
    }

    public void Show()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Instantiate lines in the leaderboard, each line is a rank/player/score entry
    /// </summary>
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

    /// <summary>
    /// Configs the textfield to stop any symbols and empty spaces from being accepted
    /// </summary>
    private void FilterInputChars(KeyDownEvent evt)
    {
        // allow nav keys
        if (evt.keyCode == KeyCode.Backspace ||
            evt.keyCode == KeyCode.Delete ||
            evt.keyCode == KeyCode.RightArrow ||
            evt.keyCode == KeyCode.LeftArrow ||
            evt.keyCode == KeyCode.Home ||
            evt.keyCode == KeyCode.End ||
            evt.keyCode == KeyCode.KeypadEnter ||
            evt.keyCode == KeyCode.Return ||
            evt.keyCode == KeyCode.Escape
            ) { return; }

        char typedChar = evt.character;

        // prevent spaces and symbols
        if (!char.IsLetterOrDigit(typedChar))
        {
            evt.StopPropagation();
        }
    }

    /// <summary>
    /// Called when player inputs a new username, should filter for profanity and update username on the backend
    /// </summary>
    /// <param name="newName">New playername</param>
    private void UsernameChanged(string newName)
    {
        if (!IsTextSafe(newName))
        {
            usernameInput.SetValueWithoutNotify(LeaderboardManager.Instance.playerName != null ? LeaderboardManager.Instance.playerName : string.Empty);
            VerifyAndAllowGameStart();
            return;
        }

        VerifyAndAllowGameStart();
        LeaderboardManager.Instance.ChangePlayerName(newName);

        if (playerScoreEntry != null)
            playerScoreEntry.text = newName;
    }

    /// <summary>
    /// Called when the player inputs a new class name, used to group players score and forms the leaderboard name
    /// </summary>
    /// <param name="newName">New class name</param>
    private void ClassNameChanged(string newName)
    {
        string difficultyName = difficultySetting.choices.ToList()[difficultySetting.value];
        string gameMapName = gameMap.choices.ToList()[gameMap.value];

        if (ComposeLeaderboardName(difficultyName, newName, gameMapName) == newName)
            return;

        if (!IsTextSafe(newName))
        {
            leaderboardNameInput.value = string.Empty;
            VerifyAndAllowGameStart();
            return;
        }

        // Clears before listing the new entries
        ClearLeaderboard();
        ShowLoadingScores();

        VerifyAndAllowGameStart();

        SettingsKeeper.Instance.HoldLevelData(difficultyName, newName, gameMapName);
        LeaderboardManager.Instance.ChangeLeaderboardName(ComposeLeaderboardName(difficultyName, newName, gameMapName));
        LeaderboardManager.Instance.GetTop10Scores();

        if(LeaderboardManager.Instance.playerScore < 0)
            LeaderboardManager.Instance.GetPlayerScore();
    }

    /// <summary>
    /// Shows a message in the leaderboard indicating that the score are loading
    /// </summary>
    void ShowLoadingScores()
    {
        uiDocument.rootVisualElement.Q<Label>("Notice").style.display = DisplayStyle.Flex;
        uiDocument.rootVisualElement.Q<Label>("Notice").text = "Carregando sala...";
    }

    /// <summary>
    /// Creates the actual leadeboard name based on classroom name, dificulty level and gameMap
    /// </summary>
    /// <param name="dificultyLevel">Name of dificulty setting in the Game Setup screen.</param>
    /// <param name="classRoomName">Name of the classroom set by the player.</param>
    /// <param name="gameMap">Name of gamemap where the game will be played.</param>
    /// <returns></returns>
    string ComposeLeaderboardName(string dificultyLevel, string classRoomName, string gameMap)
    {
        return classRoomName + "-" + gameMap + "-" + dificultyLevel;
    }

    /// <summary>
    /// Verify if the name is according to standards: no profanity, no empty spaces
    /// </summary>
    /// <param name="text">Text to be verified</param>
    /// <returns>True for safe same, False otherwise</returns>
    bool IsTextSafe(string text)
    {
        if (text.Length < 3 || text.Length > 25)
            return false;

        foreach (string s in profanityList)
        {
            if (string.Equals(s.Trim().ToLower(), text.Trim().ToLower()))
            {
                Debug.Log("profanity detected");
                return false;
            }
        }

        return true;
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
    /// Loads the game level
    /// </summary>
    private void StartGame()
    {

        SceneManager.LoadScene(gameMap.choices.ToList()[gameMap.value]);
    }

    /// <summary>
    /// Shows a message in the leaderboard indicating that it's empty
    /// </summary>
    void ShowEmptyLeaderboard()
    {
        ClearLeaderboard();

        uiDocument.rootVisualElement.Q<Label>("Notice").style.display = DisplayStyle.Flex;
        uiDocument.rootVisualElement.Q<Label>("Notice").text = "Este placar está vazio, seja o primeiro a pontuar!";
    }

    /// <summary>
    /// Prevents the player to start the game while editing username or classname
    /// </summary>
    void LockGameStartOnEdit()
    {
        startGameButton.SetEnabled(false);
    }

    /// <summary>
    /// Prevents the game from being started without a proper setup
    /// </summary>
    void VerifyAndAllowGameStart()
    {
        startGameButton.SetEnabled(leaderboardNameInput.value.Length > 0 && usernameInput.value.Length > 0);
    }

    /// <summary>
    /// Tries to prevent player's creative ways to pass the profanity protection
    /// </summary>
    public void EnhanceProfanityByVariety()
    {
        List<string> profaneWordsTemp = new List<string>(profanityList);

        foreach (string word in profaneWordsTemp)
        {
            string temp;
            temp = word.Replace("e", "3");
            profanityList.Add(temp);
            temp = word.Replace("a", "4");
            profanityList.Add(temp);
            temp = word.Replace("o", "0");
            profanityList.Add(temp);
            temp = word.Replace("e", "ee");
            profanityList.Add(temp);
            temp = word.Replace("a", "aa");
            profanityList.Add(temp);
            temp = word.Replace("a", "aaa");
            profanityList.Add(temp);
            temp = word.Replace("a", "aaaa");
            profanityList.Add(temp);
            temp = word.Replace("a", "aaaaa");
            profanityList.Add(temp);
            temp = word.Replace("l", "1");
            profanityList.Add(temp);
            temp = word.Replace("i", "1");
            profanityList.Add(temp);
            temp = word.Replace("t", "tt");
            profanityList.Add(temp);
            temp = word.Replace("e", "eee");
            profanityList.Add(temp);
            temp = word.Replace("b", "8");
            profanityList.Add(temp);
            temp = word.Replace("f", "ff");
            profanityList.Add(temp);
            temp = word.Replace("b", "3");
            profanityList.Add(temp);
            temp = word.Replace("t", "7");
            profanityList.Add(temp);
            temp = word.Replace("s", "5");
            profanityList.Add(temp);
        }
    }

    /// <summary>
    /// Removes leaderboard delegate subscriptions
    /// </summary>
    private void OnDestroy()
    {
        LeaderboardManager.Instance.OnLeaderboardReceived = null;
        LeaderboardManager.Instance.OnEmptyLeadearboardReceived = null;
        LeaderboardManager.Instance.OnSignIn = null;
    }
}
