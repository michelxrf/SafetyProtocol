using UnityEngine;

public class OnScreenControls : MonoBehaviour
{
    private CameraController mainCamera;

    private void Awake()
    {
        mainCamera = FindFirstObjectByType<CameraController>();
    }

    private void OnEnable()
    {
        mainCamera.SetupOnScreenControls();
    }
}
