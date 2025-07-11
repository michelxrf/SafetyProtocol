using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Calls a raycast on a player click or touch, the raycast is used to interact with world objects.
/// </summary>
[RequireComponent(typeof(Camera))]
public class ClickHandler : MonoBehaviour
{
    // cast clicks to clickable objects in the game world

    private Camera mainCamera;
    private PlayerControls controls;
    public bool canClick = true;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();

        // WARNING: needs testing if "canceled" will work on mobile touch
        controls.InGame.Click.canceled += OnClickPerformed;
    }

    private void OnDisable()
    {
        // WARNING: needs testing if "canceled" will work on mobile touch
        controls.InGame.Click.canceled -= OnClickPerformed;
        controls.Disable();
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
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
}