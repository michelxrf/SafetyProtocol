using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Presents the end of game report, showing player stats for the level
/// </summary>
public class GameEndScreen : MonoBehaviour
{
    UIDocument uiDocument;

    Label scoreLabel;
    Label timeLabel;
    Label accidentsLabel;
    Label hazzardsLabel;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Button>("ContinueButton").clicked += OnContinueClicked;

        scoreLabel = uiDocument.rootVisualElement.Q<Label>("Score");
        timeLabel = uiDocument.rootVisualElement.Q<Label>("Time");
        accidentsLabel = uiDocument.rootVisualElement.Q<Label>("Accidents");
        hazzardsLabel = uiDocument.rootVisualElement.Q<Label>("Hazzards");
    }


    /// <summary>
    /// Loads the data that will be shown
    /// </summary>
    /// <param name="score">Player score</param>
    /// <param name="time">Time spent on level</param>
    /// <param name="accidentsSolved">solved accidents</param>
    /// <param name="totalAccidents">total accidents on the level</param>
    /// <param name="hazzardsSolved">solved hazzards</param>
    /// <param name="totalHazzards">total hazzards on level</param>
    public void Show(int score, float time, int accidentsSolved, int totalAccidents, int hazzardsSolved, int totalHazzards)
    {
        scoreLabel.text = $"Sua pontuação: {score.ToString()}";
        timeLabel.text = $"Tempo total: {time.ToString($"#0.0")} seconds";
        accidentsLabel.text = $"Acidentes prevenidos: {accidentsSolved.ToString()}/{totalAccidents.ToString()}";
        hazzardsLabel.text = $"Riscos eliminados: {hazzardsSolved.ToString()}/{totalHazzards.ToString()}";
    }

    /// <summary>
    /// Shows the next screen: the leaderboard
    /// </summary>
    private void OnContinueClicked()
    {
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.HIGHSCORES);
    }
}
