using UnityEngine;
using UnityEngine.UIElements;

public class OnScreenControls : MonoBehaviour
{
    public UIDocument uiDocument;
    [SerializeField] CameraController mainCamera;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<CameraController>();

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
