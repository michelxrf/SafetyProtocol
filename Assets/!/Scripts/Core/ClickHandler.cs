using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Calls a raycast on a player click or touch, the raycast is used to interact with world objects.
/// </summary>
public class ClickHandler : MonoBehaviour
{
    // Singleton vars
    public static ClickHandler Instance { get; private set; }
    static bool applicationIsQuitting = false;

    [SerializeField] Camera mainCamera;
    private PlayerControls controls;
    [HideInInspector] public bool canClick = true;

    private void Awake()
    {
        if (applicationIsQuitting)
            return;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();

        controls = new PlayerControls();
        controls.Enable();
        controls.InGame.Click.canceled += OnClickPerformed;
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

    /// <summary>
    /// prevents mess wiht ghost gameobjects
    /// </summary>
    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}