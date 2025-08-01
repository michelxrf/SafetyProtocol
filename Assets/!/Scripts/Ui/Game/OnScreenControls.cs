using UnityEngine;
using UnityEngine.UIElements;

public class OnScreenControls : MonoBehaviour
{
    public UIDocument ui;
    private CameraController mainCamera;

    private void Awake()
    {
        mainCamera = FindFirstObjectByType<CameraController>();
        ui = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        mainCamera.SetupOnScreenControls();
    }

    public void Hide()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        ui.rootVisualElement.style.display = DisplayStyle.Flex;
    }

}
