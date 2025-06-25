using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UiQuizManager : MonoBehaviour
{
    // Controls the pop screen for the quiz

    [SerializeField] ClickHandler clickHandler;
    private InteractableObject associatedObject;
    private UiManager uiManager;
    private UIDocument quizUi;

    // list to keep track of player answering
    private Dictionary<VisualElement, bool> answerButtons = new();

    private void Awake()
    {
        if (clickHandler == null)
        {
            clickHandler = FindAnyObjectByType<ClickHandler>();
        }
        uiManager = transform.parent.GetComponent<UiManager>();
        quizUi = GetComponent<UIDocument>();
        quizUi.rootVisualElement.style.display = DisplayStyle.None;

        quizUi.rootVisualElement.Q<Button>("SubmitButton").clicked += OnSubmitClick;
    }

    private bool HasAtLeastOneAnswerSelected()
    {
        foreach (VisualElement answer in quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer").Children())
        {
            switch (answer)
            {
                case Toggle toggle:
                    if (toggle.value == true)
                        return true;
                    break;
                case RadioButton radio:
                    if (radio.value == true)
                        return true;
                    break;
                default:
                    break;
            }
        }

        return false;
    }

    /// <summary>
    /// The submit answer button is disabled on show, this will reenable it if at least one answer is selected.
    /// </summary>
    private void OnAnswerSelected()
    {
        quizUi.rootVisualElement.Q<Button>("SubmitButton").SetEnabled(HasAtLeastOneAnswerSelected());
    }

    /// <summary>
    /// Callback when quiz's submit button gets clicked. Verify answers and close the Quiz.
    /// </summary>
    private void OnSubmitClick()
    {
        bool isCorrect = VerifyAnswers();
        uiManager.ShowHud();
        associatedObject.OnQuizEnd(isCorrect);
        associatedObject = null;
    }

    /// <summary>
    /// verify if the player answered correctly
    /// </summary>
    private bool VerifyAnswers()
    {

        foreach(VisualElement answer in answerButtons.Keys)
        {
            bool playerAnswer;
            switch (answer)
            {
                case Toggle toggle:
                    playerAnswer = toggle.value;

                    if (answerButtons[answer] != playerAnswer)
                    {
                        return false;
                    }
                    
                    break;

                case RadioButton radio:
                    playerAnswer = radio.value;

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

    /// <summary>
    /// Loads the question data and shows it in the quiz screen.
    /// </summary>
    public void ShowQuiz(QuizQuestion questionToShow, InteractableObject interactedObject)
    {
        quizUi.rootVisualElement.style.display = DisplayStyle.Flex;

        answerButtons.Clear();

        associatedObject = interactedObject;

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

        // disables the submit answer by default until an answer is selected
        quizUi.rootVisualElement.Q<Button>("SubmitButton").SetEnabled(false);
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
        
        newButton.RegisterCallback<ChangeEvent<bool>>(evt => OnAnswerSelected());
    }

    private void AddToggleButton(string text, bool desiredAnswer)
    {
        VisualElement answersList = quizUi.rootVisualElement.Q<VisualElement>("AnswersContainer");

        Toggle newButton = new Toggle();
        newButton.AddToClassList("answers");
        newButton.text = text;
        newButton.value = false;
        newButton.Children();
        answersList.Add(newButton);
        answerButtons.Add(newButton, desiredAnswer);

        newButton.RegisterCallback<ChangeEvent<bool>>(evt => OnAnswerSelected());
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
        clickHandler.canClick = true;
        quizUi.rootVisualElement.style.display = DisplayStyle.None;
    }
}
