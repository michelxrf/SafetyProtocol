using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the information on screen like scores, time and accident alert.
/// </summary>
public class HudManager : MonoBehaviour
{
    private UIDocument uiDocument;
    private Label currentAccidentsCount;
    private Label maxAccidentsCount;
    private Label currentHazardCount;
    private Label maxHazardCount;

    private void Awake()
    {
        // gets references
        if (uiDocument ==  null)
            uiDocument = GetComponent<UIDocument>();

        uiDocument.rootVisualElement.Q<Button>("PauseButton").clicked += OnPauseClicked;

        currentAccidentsCount = uiDocument.rootVisualElement.Q<Label>("SolvedAccidents");
        maxAccidentsCount = uiDocument.rootVisualElement.Q<Label>("MaxAccidents");
        currentHazardCount = uiDocument.rootVisualElement.Q<Label>("SolvedHazards");
        maxHazardCount = uiDocument.rootVisualElement.Q<Label>("MaxHazards");

        // shows hud by default starting state
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void Start()
    {
        UpdateScores();
    }

    /// <summary>
    /// Called to update the current score when it changes.
    /// </summary>
    public void UpdateScores()
    {
        currentHazardCount.text = WorkerManager.Instance.solvedHazzards.ToString();
        currentAccidentsCount.text = WorkerManager.Instance.solvedAccidents.ToString();

        maxHazardCount.text = WorkerManager.Instance.totalHazzards.ToString();
        maxAccidentsCount.text = WorkerManager.Instance.totalAccidents.ToString();
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
