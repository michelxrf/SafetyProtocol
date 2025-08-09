using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Calls a raycast on a player click or touch, the raycast is used to interact with world objects.
/// </summary>
public class ClickHandler : MonoBehaviour
{
    // Singleton vars
    public static ClickHandler Instance { get; private set; }

    private UIDocument anyUiDocumet;
    [SerializeField] Camera mainCamera;
    private PlayerControls controls;
    [HideInInspector] public bool canClick = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            controls = new PlayerControls();
            controls.Enable();
            controls.InGame.Click.canceled += OnClickPerformed;
        }
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();

        anyUiDocumet = FindFirstObjectByType<UIDocument>();
    }

    /// <summary>
    /// Handles clicking on world objects through raycasting
    /// </summary>
    /// <param name="context"></param>
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if(!canClick)
            return;
        
        Vector2 screenPosition;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            screenPosition = Mouse.current.position.ReadValue();
        }
        else
        {
            Debug.LogError("No mouse nor touchscreen? Verify the Input bindings.");
            return;
        }

        //prevent clicking through UI
        var panel = anyUiDocumet.rootVisualElement.panel;
        Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(panel, screenPosition);
        VisualElement clickedUiElement = anyUiDocumet.rootVisualElement.panel.Pick(panelPosition);
        
        if(clickedUiElement != null)
            return;
        //

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out Clickable clickable) && canClick)
            {
                if(clickable.isEnabled)
                {
                    canClick = false;
                    clickable.OnClick();
                }
            }
        }
    }
}