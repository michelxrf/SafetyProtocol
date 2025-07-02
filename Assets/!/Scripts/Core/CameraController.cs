using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Controls camera movement.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private UIDocument onScreenInput;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private PlayerControls playerControls;
    
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int leftSideLimitX;
    [SerializeField] private int rightSideLimitX;
    [HideInInspector] public bool isMovementAllowed = true;

    private bool canMoveLeft = false;
    private bool canMoveRight = false;
    private float direction = 0f;

    private void Start()
    {
        SetupOnScreenControls();
    }

    /// <summary>
    /// Sets up references to on screen input buttons.
    /// </summary>
    public void SetupOnScreenControls()
    {
        if (onScreenInput == null)
            return;

        VisualElement root = onScreenInput.rootVisualElement;
        moveLeftButton = root.Q<Button>("MoveLeft");
        moveRightButton = root.Q<Button>("MoveRight");

        moveLeftButton.RegisterCallback<PointerDownEvent>(MoveLeftClicked, TrickleDown.TrickleDown);
        moveLeftButton.RegisterCallback<PointerUpEvent>(MoveLeftReleased, TrickleDown.TrickleDown);

        moveRightButton.RegisterCallback<PointerDownEvent>(MoveRightClicked, TrickleDown.TrickleDown);
        moveRightButton.RegisterCallback<PointerUpEvent>(MoveRightReleased, TrickleDown.TrickleDown);
    }

    private void MoveLeftClicked(PointerDownEvent evt)
    {
        // on screen control button click callback event
        direction = -1f;
    }
    private void MoveLeftReleased(PointerUpEvent evt)
    {
        // on screen control button click callback event
        direction = 0f;
    }

    private void MoveRightClicked(PointerDownEvent evt)
    {
        // on screen control button click callback event
        direction = 1f;
    }
    private void MoveRightReleased(PointerUpEvent evt)
    {
        // on screen control button click callback event
        direction = 0f;
    }

    private void OnMove(InputValue inputValue)
    {
        // get the movement direction from the Input System

        direction = inputValue.Get<float>();
    }
    private void Update()
    {
        MoveCamera();
    }

    /// <summary>
    /// does the actual camera movement if allowed
    /// </summary>
    private void MoveCamera()
    {
        if (!isMovementAllowed) // prevents camera movement during events
            return;
        
        if (direction == 0f) // skip if not trying to move
            return;

        VerifyBounds();

        if ((!canMoveLeft && direction < 0f) || (!canMoveRight && direction > 0f)) // skip if out of bounds
            return;

        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x + speed * direction * Time.deltaTime, playerCamera.transform.position.y, playerCamera.transform.position.z);
    }

    /// <summary>
    /// prevents the camera from moving beyond set bounds.
    /// </summary>
    private void VerifyBounds()
    {
        // limits the camera movement to the bounds defined by the designer

        canMoveLeft = playerCamera.transform.position.x > leftSideLimitX;
        moveLeftButton.SetEnabled(canMoveLeft);

        canMoveRight = playerCamera.transform.position.x < rightSideLimitX;
        moveRightButton.SetEnabled(canMoveRight);
    }


}
