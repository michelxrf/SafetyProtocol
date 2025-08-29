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

    private void Awake()
    {
        ui = GetComponent<UIDocument>();

        ui.rootVisualElement.Q<Button>("Multiplayer").clicked += ShowGameSetup;
        ui.rootVisualElement.Q<Button>("Settings").clicked += ShowSettings;
    }


    private void ShowSettings()
    {
        Hide();
        settingsScreen.Show();
    }

    /// <summary>
    /// Shows the game set up screen
    /// </summary>
    private void ShowGameSetup()
    {
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
