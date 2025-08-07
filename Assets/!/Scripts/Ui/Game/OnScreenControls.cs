using UnityEngine;
using UnityEngine.UIElements;

public class OnScreenControls : MonoBehaviour
{
    public UIDocument uiDocument;
    private CameraController mainCamera;

    private void Awake()
    {
        mainCamera = FindFirstObjectByType<CameraController>();
        uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        mainCamera.SetupOnScreenControls();
    }

    public void Hide()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

}
