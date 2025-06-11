using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UiQuizManager : MonoBehaviour
{
    // TODO: shuffle answers order

    private UiManager uiManager;
    private UIDocument quizUi;

    // list to keep track of player answering
    private Dictionary<VisualElement, bool> answerButtons = new();

    private void Awake()
    {
        uiManager = transform.parent.GetComponent<UiManager>();
        quizUi = GetComponent<UIDocument>();
        transform.gameObject.SetActive(false);
    }

    private void OnSubmitClick()
    {
        // callback when quiz's submit button gets clicked

        bool isCorrect = VerifyAnswers();
        Debug.Log($"Quiz answered correctly? {isCorrect}");
        uiManager.ShowHud();
    }

    private bool VerifyAnswers()
    {
        // verify if the player answered correctly
        // BUGGED AS FUCK

        //foreach
        Debug.Log(answerButtons);

        foreach(VisualElement answer in answerButtons.Keys)
        {
            bool playerAnswer;
            switch (answer)
            {
                case Toggle toggle:
                    playerAnswer = toggle.value;

                    Debug.Log($"{answerButtons[answer]} / {playerAnswer}");

                    if (answerButtons[answer] != playerAnswer)
                    {
                        return false;
                    }
                    
                    break;

                case RadioButton radio:
                    playerAnswer = radio.value;

                    Debug.Log($"{answerButtons[answer]} / {playerAnswer}");

                    if (answerButtons[answer] != playerAnswer)
                    {

                        return false;
                    }

                    break;

                default:
                    break;
            }
        }

        return true;
    }

    public void ShowQuiz(QuizQuestion questionToShow)
    {
        // the questions and answers according to qeustion data
        transform.gameObject.SetActive(true);

        quizUi.rootVisualElement.Q<Label>("Question").text = questionToShow.question;
        quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer").Clear();

        // use toggle buttons for multiple right answers
        if (questionToShow.rightAnswers.Length > 1)
        {
            quizUi.rootVisualElement.Q<Label>("Instruction").text = "Marque todas as respostas corretas.";
            foreach (var answer in questionToShow.rightAnswers)
            {
                AddToggleButton(answer, true);
            }
            foreach (var answer in questionToShow.wrongAnswers)
            {
                AddToggleButton(answer, false);
            }
        }

        // use radio buttons for single right answers
        else
        {
            quizUi.rootVisualElement.Q<Label>("Instruction").text = "Marque apenas uma resposta.";
            foreach (var answer in questionToShow.rightAnswers)
            {
                AddRadioButton(answer, true);
            }
            foreach (var answer in questionToShow.wrongAnswers)
            {
                AddRadioButton(answer, false);
            }
        }

        ShuffleAnswers(quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer"));

        quizUi.rootVisualElement.Q<Button>("SubmitButton").clicked += OnSubmitClick;
    }

    private void AddRadioButton(string text, bool desiredAnswer)
    {
        VisualElement answersList = quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer");

        RadioButton newButton = new RadioButton();
        newButton.text = text;
        newButton.value = false;
        newButton.AddToClassList("answers");
        answersList.Add(newButton);
        
        answerButtons.Add(newButton, desiredAnswer);
    }

    private void AddToggleButton(string text, bool desiredAnswer)
    {
        VisualElement answersList = quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer");

        Toggle newButton = new Toggle();
        newButton.text = text;
        newButton.value = false;
        newButton.AddToClassList("answers");
        answersList.Add(newButton);

        answerButtons.Add(newButton, desiredAnswer);
    }

    private void ShuffleAnswers(VisualElement answerContainer)
    {
        // Reorders the order the questions show

        List<VisualElement> elements = answerContainer.Children().ToList();

        // Shuffle the list of elements
        for (int i = elements.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (elements[i], elements[j]) = (elements[j], elements[i]);
        }

        // Re-insert elements in the shuffled order
        answerContainer.Clear();
        foreach (var el in elements)
        {
            answerContainer.Add(el);
        }
    }


    public void HideQuiz()
    {
        transform.gameObject.SetActive(false);
    }
}
