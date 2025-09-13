using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Controls the pause menu.
/// </summary>
public class PauseScreen : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        uiDocument.rootVisualElement.Q<Button>("UnpauseButton").clicked += UnpauseClicked;
        uiDocument.rootVisualElement.Q<Button>("UnpauseButton").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));

        uiDocument.rootVisualElement.Q<Button>("RestartButton").clicked += RestartGame;
        uiDocument.rootVisualElement.Q<Button>("RestartButton").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));

        uiDocument.rootVisualElement.Q<Button>("QuitButton").clicked += QuitClicked;
        uiDocument.rootVisualElement.Q<Button>("QuitButton").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));
    }

    /// <summary>
    /// Hides the pause screen.
    /// </summary>
    private void UnpauseClicked()
    {
        WorkerManager.Instance.UnpauseGame();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
    }

    /// <summary>
    /// Back to main menu
    /// </summary>
    private void QuitClicked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Loads the game level
    /// </summary>
    private void RestartGame()
    {
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
        SceneManager.LoadScene(SettingsKeeper.Instance.gameMap);
    }
}
