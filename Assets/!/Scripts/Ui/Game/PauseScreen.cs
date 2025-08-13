using UnityEngine;
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

        uiDocument.rootVisualElement.Q<Button>("QuitButton").clicked += QuitClicked;
    }

    /// <summary>
    /// Hides the pause screen.
    /// </summary>
    private void UnpauseClicked()
    {
        WorkerManager.Instance.UnpauseGame();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
    }

    /// <summary>
    /// Back to main menu
    /// </summary>
    private void QuitClicked()
    {
        SceneManager.LoadScene(0);
    }
}
