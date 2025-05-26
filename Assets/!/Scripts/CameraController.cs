using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // controls the camera movement

    // TODO: verify game state (pause, gameover, play) to prevent movement outside of play

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject moveLeftButton;
    [SerializeField] private GameObject moveRightButton;
    [SerializeField] private PlayerControls playerControls;
    
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int leftSideLimitX;
    [SerializeField] private int rightSideLimitX;

    private bool canMoveLeft = false;
    private bool canMoveRight = false;
    private float direction = 0f;

    Vector3 onClickPos;

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        // does the actual camera movement if allowed

        if (direction == 0f) // skip if not trying to move
            return;

        VerifyBounds();

        if ((!canMoveLeft && direction < 0f) || (!canMoveRight && direction > 0f)) // skip if out of bounds
            return;

        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x + speed * direction * Time.deltaTime, playerCamera.transform.position.y, playerCamera.transform.position.z);
    }

    private void VerifyBounds()
    {
        // limits the camera movement to the bounds defined by the designer

        canMoveLeft = playerCamera.transform.position.x > leftSideLimitX;
        moveLeftButton.SetActive(canMoveLeft);

        canMoveRight = playerCamera.transform.position.x < rightSideLimitX;
        moveRightButton.SetActive(canMoveRight);
    }

    private void OnMove(InputValue inputValue)
    {
        // get the movement direction from the Input System

        direction = inputValue.Get<float>();
    }
}
