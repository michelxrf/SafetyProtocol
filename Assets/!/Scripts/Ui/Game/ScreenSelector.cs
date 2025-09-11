using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the showing and hiding of the different in game screens
/// </summary>
public class ScreenSelector : MonoBehaviour
{
    // Singleotn vars
    public static ScreenSelector Instance { get; private set; }

    public enum SCREENMODE { GAME, PAUSE, ACCIDENT, GAMEEND, HIGHSCORES, QUIZ }
    public SCREENMODE currentScreenMode {get; private set; } = SCREENMODE.GAME;

    [Header("References")]
    [SerializeField] UIDocument onScreenControls;
    [SerializeField] UIDocument quiz;
    [SerializeField] UIDocument hud;
    [SerializeField] UIDocument pause;
    [SerializeField] UIDocument accident;
    [SerializeField] UIDocument gameEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SwitchScreen(SCREENMODE.GAME);
    }

    /// <summary>
    /// Toggles the screens on and off according to desired game mode
    /// </summary>
    /// <param name="newMode"></param>
    public void SwitchScreen(SCREENMODE newMode)
    {
        currentScreenMode = newMode;

        switch (newMode)
        {
            case SCREENMODE.GAME:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.Flex;
                quiz.rootVisualElement.style.display = DisplayStyle.None;
                hud.rootVisualElement.style.display = DisplayStyle.Flex;
                pause.rootVisualElement.style.display = DisplayStyle.None;
                accident.rootVisualElement.style.display = DisplayStyle.None;
                gameEnd.rootVisualElement.style.display = DisplayStyle.None;

                ClickHandler.Instance.canClick = true;

                break;
            case SCREENMODE.PAUSE:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.None;
                quiz.rootVisualElement.style.display = DisplayStyle.None;
                hud.rootVisualElement.style.display = DisplayStyle.None;
                pause.rootVisualElement.style.display = DisplayStyle.Flex;
                accident.rootVisualElement.style.display = DisplayStyle.None;
                gameEnd.rootVisualElement.style.display = DisplayStyle.None;

                ClickHandler.Instance.canClick = false;

                break;

            case SCREENMODE.QUIZ:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.None;
                quiz.rootVisualElement.style.display = DisplayStyle.Flex;
                hud.rootVisualElement.style.display = DisplayStyle.None;
                pause.rootVisualElement.style.display = DisplayStyle.None;
                accident.rootVisualElement.style.display = DisplayStyle.None;
                gameEnd.rootVisualElement.style.display = DisplayStyle.None;

                ClickHandler.Instance.canClick = false;
                break;

            case SCREENMODE.ACCIDENT:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.None;
                quiz.rootVisualElement.style.display = DisplayStyle.None;
                hud.rootVisualElement.style.display = DisplayStyle.None;
                pause.rootVisualElement.style.display = DisplayStyle.None;
                accident.rootVisualElement.style.display = DisplayStyle.Flex;
                gameEnd.rootVisualElement.style.display = DisplayStyle.None;

                ClickHandler.Instance.canClick = false;
                break;
            case SCREENMODE.GAMEEND:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.None;
                quiz.rootVisualElement.style.display = DisplayStyle.None;
                hud.rootVisualElement.style.display = DisplayStyle.None;
                pause.rootVisualElement.style.display = DisplayStyle.None;
                accident.rootVisualElement.style.display = DisplayStyle.None;
                gameEnd.rootVisualElement.style.display = DisplayStyle.Flex;

                ClickHandler.Instance.canClick = false;
                break;
            case SCREENMODE.HIGHSCORES:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.None;
                quiz.rootVisualElement.style.display = DisplayStyle.None;
                hud.rootVisualElement.style.display = DisplayStyle.None;
                pause.rootVisualElement.style.display = DisplayStyle.None;
                accident.rootVisualElement.style.display = DisplayStyle.None;
                gameEnd.rootVisualElement.style.display = DisplayStyle.None;

                ClickHandler.Instance.canClick = false;
                break;
            default:
                onScreenControls.rootVisualElement.style.display = DisplayStyle.None;
                quiz.rootVisualElement.style.display = DisplayStyle.None;
                hud.rootVisualElement.style.display = DisplayStyle.None;
                pause.rootVisualElement.style.display = DisplayStyle.None;
                accident.rootVisualElement.style.display = DisplayStyle.None;
                gameEnd.rootVisualElement.style.display = DisplayStyle.None;
                
                Debug.LogError("invalid screen mode.");
                ClickHandler.Instance.canClick = false;
                break;
        }  

    }
}
