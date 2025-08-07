using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the pause menu.
/// </summary>
public class PauseScreen : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [SerializeField] private WorkerManager workerManager;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (workerManager == null)
            workerManager = FindFirstObjectByType<WorkerManager>();

        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        uiDocument.rootVisualElement.Q<Button>("UnpauseButton").clicked += UnpauseClicked;
    }

    /// <summary>
    /// Hides the pause screen.
    /// </summary>
    private void UnpauseClicked()
    {
        WorkerManager.Instance.UnpauseGame();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
    }
}
