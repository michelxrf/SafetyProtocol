using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


/// <summary>
/// Displays the leaderboard at the end of the level
/// </summary>
public class GameEndLeaderboard : MonoBehaviour
{
    UIDocument uiDocument;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Button>("ContinueButton").clicked += OnBackToMenuClicked;
    }

    private void OnBackToMenuClicked()
    {
        SceneManager.LoadScene(0);
    }
}
