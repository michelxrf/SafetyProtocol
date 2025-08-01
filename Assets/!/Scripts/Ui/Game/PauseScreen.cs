using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the pause menu.
/// </summary>
public class PauseScreen : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private HudManager hud;
    [SerializeField] private OnScreenControls onScreenControls;
    [SerializeField] private WorkerManager workerManager;

    private void Awake()
    {
        if (ui == null)
            ui = GetComponent<UIDocument>();

        if (hud == null)
            hud = FindFirstObjectByType<HudManager>();

        if (workerManager == null)
            workerManager = FindFirstObjectByType<WorkerManager>();

        if (onScreenControls == null)
            onScreenControls = FindFirstObjectByType<OnScreenControls>();

        ui.rootVisualElement.style.display = DisplayStyle.None;
        ui.rootVisualElement.Q<Button>("UnpauseButton").clicked += Hide;
    }

    /// <summary>
    /// Shows the pause screen.
    /// </summary>
    public void Show()
    {
        ui.rootVisualElement.style.display = DisplayStyle.Flex;

        onScreenControls.Hide();
        hud.Hide();
    }

    /// <summary>
    /// Hides the pause screen.
    /// </summary>
    private void Hide()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
        hud.Show();
        onScreenControls.Show();
    }
}
