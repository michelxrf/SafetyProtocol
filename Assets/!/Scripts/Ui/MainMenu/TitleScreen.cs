using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the beahvior of the title screen
/// </summary>
public class TitleScreen : MonoBehaviour
{
    UIDocument ui;
    [SerializeField] GameSetup gameSetupScreen;
    [SerializeField] SettingsScreen settingsScreen;
    [SerializeField] TutorialScreen tutorialScreen;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();

        ui.rootVisualElement.Q<Button>("Multiplayer").clicked += ShowGameSetup;
        ui.rootVisualElement.Q<Button>("Multiplayer").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));

        ui.rootVisualElement.Q<Button>("Settings").clicked += ShowSettings;
        ui.rootVisualElement.Q<Button>("Settings").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));

        ui.rootVisualElement.Q<Button>("Tutorial").clicked += ShowTutorial;
        ui.rootVisualElement.Q<Button>("Tutorial").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));
    }

    private void Start()
    {
        if (SettingsKeeper.Instance.classRoomName != null)
            ShowGameSetup();
    }

    private void ShowTutorial()
    {
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
        Hide();
        tutorialScreen.Show();
    }

    private void ShowSettings()
    {
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
        Hide();
        settingsScreen.Show();
    }

    /// <summary>
    /// Shows the game set up screen
    /// </summary>
    private void ShowGameSetup()
    {
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
        Hide();
        gameSetupScreen.Show();
    }

    public void Show()
    {
        ui.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
    }
}
