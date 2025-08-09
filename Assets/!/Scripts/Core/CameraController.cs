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
    [SerializeField] private Button moveForwardButton;
    [SerializeField] private Button moveBackButton;
    [SerializeField] private PlayerControls playerControls;
    
    [Header("Settings")]
    [SerializeField] private float speed = 10f;

    [SerializeField] private Transform cameraLimiter1;
    [SerializeField] private Transform cameraLimiter2;

    private float leftSideLimitX;
    private float rightSideLimitX;
    private float forwardLimitZ;
    private float backLimitZ;

    [HideInInspector] public bool isMovementAllowed = true;

    private bool canMoveLeft = false;
    private bool canMoveRight = false;
    private bool canMoveForward = false;
    private bool canMoveBackward = false;

    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        CalculeMapBorders();
    }

    /// <summary>
    /// Calculates camera movement limits based on two limiters points
    /// </summary>
    void CalculeMapBorders()
    {
        leftSideLimitX = Mathf.Min(cameraLimiter1.transform.position.x, cameraLimiter2.transform.position.x);
        rightSideLimitX = Mathf.Max(cameraLimiter1.transform.position.x, cameraLimiter2.transform.position.x);

        backLimitZ = Mathf.Min(cameraLimiter1.transform.position.z, cameraLimiter2.transform.position.z);
        forwardLimitZ = Mathf.Max(cameraLimiter1.transform.position.z, cameraLimiter2.transform.position.z);
    }

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
            onScreenInput = FindFirstObjectByType<OnScreenControls>().GetComponent<UIDocument>();
        
        VisualElement root = onScreenInput.rootVisualElement;

        moveLeftButton = root.Q<Button>("MoveLeft");
        moveRightButton = root.Q<Button>("MoveRight");
        moveForwardButton = root.Q<Button>("Forward");
        moveBackButton = root.Q<Button>("Back");

        moveForwardButton.RegisterCallback<PointerDownEvent>(ForwardClicked, TrickleDown.TrickleDown);
        moveForwardButton.RegisterCallback<PointerUpEvent>(ForwardReleased, TrickleDown.TrickleDown);

        moveBackButton.RegisterCallback<PointerDownEvent>(BackClicked, TrickleDown.TrickleDown);
        moveBackButton.RegisterCallback<PointerUpEvent>(BackReleased, TrickleDown.TrickleDown);

        moveLeftButton.RegisterCallback<PointerDownEvent>(MoveLeftClicked, TrickleDown.TrickleDown);
        moveLeftButton.RegisterCallback<PointerUpEvent>(MoveLeftReleased, TrickleDown.TrickleDown);

        moveRightButton.RegisterCallback<PointerDownEvent>(MoveRightClicked, TrickleDown.TrickleDown);
        moveRightButton.RegisterCallback<PointerUpEvent>(MoveRightReleased, TrickleDown.TrickleDown);
    }

    private void ForwardClicked(PointerDownEvent evt)
    {
        // on screen control button click callback event
        direction.z = 1f;
    }
    private void ForwardReleased(PointerUpEvent evt)
    {
        // on screen control button click callback event
        direction.z = 0f;
    }

    private void BackClicked(PointerDownEvent evt)
    {
        // on screen control button click callback event
        direction.z = -1f;
    }
    private void BackReleased(PointerUpEvent evt)
    {
        // on screen control button click callback event
        direction.z = 0f;
    }

    private void MoveLeftClicked(PointerDownEvent evt)
    {
        // on screen control button click callback event
        direction.x = -1f;
    }
    private void MoveLeftReleased(PointerUpEvent evt)
    {
        // on screen control button click callback event
        direction.x = 0f;
    }

    private void MoveRightClicked(PointerDownEvent evt)
    {
        // on screen control button click callback event
        direction.x = 1f;
    }
    private void MoveRightReleased(PointerUpEvent evt)
    {
        // on screen control button click callback event
        direction.x = 0f;
    }

    private void OnMove(InputValue inputValue)
    {
        // get the movement direction from the Input System

        direction = inputValue.Get<Vector3>();
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
        
        if (direction.magnitude == 0f) // skip if not trying to move
            return;

        VerifyBounds();

        if ((!canMoveLeft && direction.x < 0f) || (!canMoveRight && direction.x > 0f)) // skip if out of bounds
            direction.x = 0f;

        if ((!canMoveBackward && direction.z < 0f) || (!canMoveForward && direction.z > 0f)) // skip if out of bounds
            direction.z = 0f;

        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x + speed * direction.x * Time.deltaTime,
                playerCamera.transform.position.y,
                playerCamera.transform.position.z + speed * direction.z * Time.deltaTime);
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

        canMoveForward = playerCamera.transform.position.z < forwardLimitZ;
        moveForwardButton.SetEnabled(canMoveForward);

        canMoveBackward = playerCamera.transform.position.z > backLimitZ;
        moveBackButton.SetEnabled(canMoveBackward);
    }


}
