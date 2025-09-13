using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Controls the behavior of the tutorial screen
/// </summary>
public class TutorialScreen : MonoBehaviour
{
    [SerializeField] UIDocument ui;
    [SerializeField] TitleScreen titleScreen;

    private Label pageTittle;
    private Label pageBody;
    private Label pageIndex;

    [SerializeField] TutorialPage[] pages;
    private int currentIndex;

    private void Awake()
    {
        if (ui == null)
            ui = GetComponent<UIDocument>();

        if (titleScreen == null)
            titleScreen = FindFirstObjectByType<TitleScreen>();

        ui.rootVisualElement.style.display = DisplayStyle.None;

        pageTittle = ui.rootVisualElement.Q<Label>("Title");
        pageBody = ui.rootVisualElement.Q<Label>("Body");
        pageIndex = ui.rootVisualElement.Q<Label>("Index");

        ui.rootVisualElement.Q<Button>("Next").clicked += OnNextPageClicked;
        ui.rootVisualElement.Q<Button>("Menu").clicked += OnBackClicked;

        if (pages.Length < 1)
            Debug.LogError("No tutorial pages set");
    }

    /// <summary>
    /// Called by the other screens to show this screen
    /// </summary>
    public void Show()
    {
        currentIndex = 0;

        ShowPage(currentIndex);
        ui.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Load text from a page scriptable into the screen
    /// </summary>
    /// <param name="index">pages array index</param>
    private void ShowPage(int index)
    {
        currentIndex = index;

        pageBody.text = pages[currentIndex].body;
        pageTittle.text = pages[currentIndex].title;
        pageIndex.text = $"{currentIndex + 1} / {pages.Length}";
    }

    /// <summary>
    /// Callback to hide this scrren
    /// </summary>
    private void OnBackClicked()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
        titleScreen.Show();
    }

    /// <summary>
    /// Handles page changing
    /// </summary>
    private void OnNextPageClicked()
    {
        currentIndex = (int)Mathf.Repeat(currentIndex + 1, pages.Length);
        ShowPage(currentIndex);
    }
}
