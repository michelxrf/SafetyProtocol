using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the information on screen like scores, time and accident alert.
/// </summary>
public class HudManager : MonoBehaviour
{
    private UIDocument uiDocument;
    private Label accidentsCount;
    private Label hazardCount;
    private Label gameTime;
    private Button pauseButton;

    private void Awake()
    {
        // gets references
        if (uiDocument ==  null)
            uiDocument = GetComponent<UIDocument>();

        pauseButton = uiDocument.rootVisualElement.Q<Button>("PauseButton");

        if(pauseButton != null )
            pauseButton.clicked += OnPauseClicked;

        accidentsCount = uiDocument.rootVisualElement.Q<Label>("SolvedAccidents");
        hazardCount = uiDocument.rootVisualElement.Q<Label>("SolvedHazards");
        gameTime = uiDocument.rootVisualElement.Q<Label>("GameTimeLabel");

        // shows hud by default starting state
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void Start()
    {
        UpdateScores();
        UpdateTime();
    }

    private void Update()
    {
        UpdateTime();
    }

    /// <summary>
    /// Shows current game time
    /// </summary>
    private void UpdateTime()
    {
        int minutes = Mathf.FloorToInt(WorkerManager.Instance.gameTime / 60);
        int seconds = Mathf.FloorToInt(WorkerManager.Instance.gameTime % 60);

        gameTime.text = string.Format("{00:00}:{01:00}", minutes, seconds);
    }

    /// <summary>
    /// Called to update the current score when it changes.
    /// </summary>
    public void UpdateScores()
    {
        hazardCount.text = WorkerManager.Instance.solvedHazzards.ToString() + "/" + WorkerManager.Instance.totalHazzards.ToString();
        accidentsCount.text = WorkerManager.Instance.solvedAccidents.ToString() + "/" + WorkerManager.Instance.totalAccidents.ToString();
    }

    /// <summary>
    /// Calls a game wide pause.
    /// </summary>
    private void OnPauseClicked()
    {
        WorkerManager.Instance.PauseGame();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.PAUSE);
    }

}
