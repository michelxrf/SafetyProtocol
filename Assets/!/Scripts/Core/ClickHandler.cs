using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class ClickHandler : MonoBehaviour
{
    private Camera mainCamera;
    private PlayerControls controls;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.InGame.Click.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        controls.InGame.Click.performed -= OnClickPerformed;
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
            if (hit.collider.TryGetComponent(out Clickable clickable))
            {
                clickable.OnClick();
            }
        }
    }
}