using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

/// <summary>
/// Manages the information on screen like scores, time and accident alert.
/// </summary>
public class HudManager : MonoBehaviour
{
    private WorkerManager workerManager;
    private PauseScreen pauseScreen;

    private UIDocument hud;
    private VisualElement accidentAlertIcon;
    private Label accidentCountdown;
    private Label currentAccidentsCount;
    private Label maxAccidentsCount;
    private Label currentHazardCount;
    private Label maxHazardCount;
    private Button pauseButton;

    [SerializeField] float alertBlinkInterval = 1f;

    private void Awake()
    {
        if (hud ==  null)
            hud = GetComponent<UIDocument>();

        hud.rootVisualElement.style.display = DisplayStyle.Flex;

        pauseButton = hud.rootVisualElement.Q<Button>("PauseButton");

        accidentAlertIcon = hud.rootVisualElement.Q<VisualElement>("AlertIcon");
        accidentAlertIcon.style.display = DisplayStyle.None;

        accidentCountdown = hud.rootVisualElement.Q<Label>("Countdown");
        accidentCountdown.style.display = DisplayStyle.None;

        currentAccidentsCount = hud.rootVisualElement.Q<Label>("SolvedAccidents");
        maxAccidentsCount = hud.rootVisualElement.Q<Label>("MaxAccidents");
        
        currentHazardCount = hud.rootVisualElement.Q<Label>("SolvedHazards");
        maxHazardCount = hud.rootVisualElement.Q<Label>("MaxHazards");

        if (workerManager == null)
            workerManager = FindFirstObjectByType<WorkerManager>();

        if (pauseScreen == null)
            pauseScreen = FindFirstObjectByType<PauseScreen>();
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
        currentHazardCount.text = workerManager.solvedHazzards.ToString();
        currentAccidentsCount.text = workerManager.solvedAccidents.ToString();

        maxHazardCount.text = workerManager.totalHazzards.ToString();
        maxAccidentsCount.text = workerManager.totalAccidents.ToString();
    }

    private void OnEnable()
    {
        pauseButton.clicked += OnPauseClicked;
    }

    private void OnDisable()
    {
        pauseButton.clicked -= OnPauseClicked;
    }

    public void UpdateAccidentCountdown(float remainingTime)
    {
        accidentCountdown.text = string.Format($"{remainingTime:F1} s");
    }

    public void ShowAlert(float remainingTime)
    {
        UpdateAccidentCountdown(remainingTime);
        accidentCountdown.style.display = DisplayStyle.Flex;

        StartCoroutine(BlinkAlert());
    }

    public void Show()
    {
        hud.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        hud.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void HideAlert()
    {
        StopAllCoroutines();
        accidentCountdown.style.display = DisplayStyle.None;
        accidentAlertIcon.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Calls a game wide pause.
    /// </summary>
    private void OnPauseClicked()
    {
        workerManager.PauseGame();
        pauseScreen.Show();
    }

    /// <summary>
    /// Blinks the alert on and off.
    /// </summary>
    private IEnumerator BlinkAlert()
    {
        while (true)
        {
            if(accidentAlertIcon.style.display == DisplayStyle.Flex)
            {
                accidentAlertIcon.style.display = DisplayStyle.None;
            }
            else
            {
                accidentAlertIcon.style.display = DisplayStyle.Flex;
            }

            yield return new WaitForSeconds(alertBlinkInterval);
        }
    }
   
}
