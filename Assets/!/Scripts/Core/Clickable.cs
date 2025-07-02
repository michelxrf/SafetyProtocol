using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Clickable : MonoBehaviour
{
    // makes the object reacts to a click
    // IDEAS: make the object play a sfx once clicked

    public QuizQuestion questionData;
    private UiQuizManager quizScreen;
    [HideInInspector] public bool isEnabled = true;

    private void Awake()
    {
        quizScreen = FindFirstObjectByType<UiQuizManager>();
    }

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
