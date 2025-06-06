using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    // handles switching UI elements as game changes state
    // TODO: shuffle answers order

    [SerializeField] private UIDocument pauseMenu;
    [SerializeField] private UIDocument quizUi;
    [SerializeField] private UIDocument hud;

    private void Awake()
    {
        //pauseMenu.enabled = false;
        quizUi.enabled = false;
        hud.enabled = true;
    }

    public void ShowQuiz(QuizQuestion questionToShow)
    {
        // show the quiz screen and hides all that should not appear
        // and sets the questions and answers according to qeustion data

        quizUi.rootVisualElement.Q<Label>("Question").text = questionToShow.question;
        quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer").Clear();


        // use toggle buttons for multiple right answers
        if (questionToShow.rightAnswers.Length > 1)
        {
            quizUi.rootVisualElement.Q<Label>("Instruction").text = "Marque todas as respostas corretas.";
            foreach (var answer in questionToShow.rightAnswers)
            {
                AddToggleButton(answer);
            }
            foreach (var answer in questionToShow.wrongAnswers)
            {
                AddToggleButton(answer);
            }
        }

        // use radio buttons for single right answers
        else
        {
            quizUi.rootVisualElement.Q<Label>("Instruction").text = "Marque apenas uma resposta.";
            foreach (var answer in questionToShow.rightAnswers)
            {
                AddRadioButton(answer);
            }
            foreach (var answer in questionToShow.wrongAnswers)
            {
                AddRadioButton(answer);
            }
        }

        quizUi.enabled = true;
    }

    private void AddRadioButton(string text)
    {
        VisualElement answersList = quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer");

        RadioButton newButton = new RadioButton();
        newButton.text = text;
        newButton.AddToClassList("answers");
        answersList.Add(newButton);
    }

    private void AddToggleButton(string text)
    {
        VisualElement answersList = quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer");

        Toggle newButton = new Toggle();
        newButton.text = text;
        newButton.AddToClassList("answers");
        answersList.Add(newButton);
    }
}
