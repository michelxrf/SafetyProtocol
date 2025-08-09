using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Allows objects to respond to clicks or touches.
/// </summary>
public class Clickable : MonoBehaviour
{
    public QuizQuestion questionData;
    private UiQuizManager quizScreen;
    [HideInInspector] public bool isEnabled = true;

    private void Start()
    {
        quizScreen = FindFirstObjectByType<UiQuizManager>();
    }

    /// <summary>
    /// Shows associated quiz once object is clicked.
    /// </summary>
    public void OnClick()
    {
        if (!isEnabled)
            return;

        if (questionData != null)
        {
            quizScreen.ShowQuiz(questionData, GetComponent<InteractableObject>());
        }
    }
}
