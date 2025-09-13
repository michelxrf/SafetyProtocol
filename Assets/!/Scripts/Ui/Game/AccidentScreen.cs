using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the accident screen, showing the player they just failed an accident quiz.
/// </summary>
public class AccidentScreen : MonoBehaviour
{
    private UIDocument uiDocument;
    [SerializeField] AudioClip accidentSfx;

    private void Awake()
    {
        // gets references
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;

        uiDocument.rootVisualElement.Q<Button>("Continue").clicked += ContinueClicked;
        uiDocument.rootVisualElement.Q<Button>("Continue").RegisterCallback<MouseEnterEvent>(evt =>
            AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.HOVER, transform));
    }

    /// <summary>
    /// Loads the accident data on screen and shows it.
    /// </summary>
    /// <param name="accidentData">The accident that just happened data.</param>
    public void Show(AccidentData accidentData)
    {
        // sets the accident screen
        uiDocument.rootVisualElement.Q<Label>("Tittle").text = accidentData.accidentTitle;
        uiDocument.rootVisualElement.Q<Label>("Description").text = accidentData.accidentDescription;
        uiDocument.rootVisualElement.Q<Label>("Description").style.whiteSpace = WhiteSpace.Normal;

        if (accidentData.accidentImage != null)
            uiDocument.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage = accidentData.accidentImage;

        // play the SFX
        if (accidentData.accidentAudio != null)
        {
            AudioManager.Instance.PlaySFX(accidentData.accidentAudio, transform);
        }
        else
        {
            AudioManager.Instance.PlaySFX(accidentSfx, transform);
        }

        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.ACCIDENT);
    }

    /// <summary>
    /// Hides the accident screen and continues the game
    /// </summary>
    public void ContinueClicked()
    {
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
        WorkerManager.Instance.CallNextAccident();
        AudioManager.Instance.PlaySFX(AudioManager.DEFAULT_UISFX.CLICK, transform);
    }
}
