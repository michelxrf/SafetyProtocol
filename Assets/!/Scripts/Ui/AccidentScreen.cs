using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the accident screen, showing the player they just failed an accident quiz.
/// </summary>
public class AccidentScreen : MonoBehaviour
{
    private UIDocument ui;
    private HudManager hudManager;
    private AudioSource accidentSfx;

    private void Awake()
    {
        // gets references
        ui = GetComponent<UIDocument>();
        ui.rootVisualElement.style.display = DisplayStyle.None;

        ui.rootVisualElement.Q<Button>("Continue").clicked += Hide;

        hudManager = FindFirstObjectByType<HudManager>();
    }

    /// <summary>
    /// Loads the accident data on screen and shows it.
    /// </summary>
    /// <param name="accidentData">The accident that just happened data.</param>
    public void Show(AccidentData accidentData)
    {
        // sets the accident screen
        ui.rootVisualElement.Q<Label>("Tittle").text = accidentData.accidentTitle;
        ui.rootVisualElement.Q<Label>("Description").text = accidentData.accidentDescription;
        
        if (accidentData.accidentImage != null)
            ui.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage = accidentData.accidentImage;

        // play the SFX
        if (accidentData.accidentAudio != null)
        {
            accidentSfx.clip = accidentData.accidentAudio;
            accidentSfx.Play();
        }

        // shows it
        ui.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Hides the accident screen.
    /// </summary>
    public void Hide()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
        hudManager.Show();
    }
}
