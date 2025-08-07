using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the pop up screen for the quiz
/// </summary>
public class UiQuizManager : MonoBehaviour
{
    private InteractableObject associatedObject;
    private UIDocument uiDocument;

    // list to keep track of player answering
    private Dictionary<VisualElement, bool> answerButtons = new();

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;

        uiDocument.rootVisualElement.Q<Button>("SubmitButton").clicked += OnSubmitClicked;
        uiDocument.rootVisualElement.Q<Button>("CloseButton").clicked += OnCloseClicked;
    }

    /// <summary>
    /// Prevents the submit button from being clicked if not a single answer has been selected.
    /// </summary>
    /// <returns>True if more than zero answers are selected, false otherwise.</returns>
    private bool HasAtLeastOneAnswerSelected()
    {
        foreach (VisualElement answer in uiDocument.rootVisualElement.Q<VisualElement>("AnswersContainer").Children())
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
        uiDocument.rootVisualElement.Q<Button>("SubmitButton").SetEnabled(HasAtLeastOneAnswerSelected());
    }

    /// <summary>
    /// Callback when quiz's submit button gets clicked. Verify answers and close the Quiz.
    /// </summary>
    private void OnSubmitClicked()
    {
        bool isCorrect = VerifyAnswers();
        associatedObject.OnQuizEnd(isCorrect);
        associatedObject = null;

        answerButtons.Clear();
    }

    /// <summary>
    /// verify if the player answered correctly
    /// </summary>
    private bool VerifyAnswers()
    {
        foreach (VisualElement answer in answerButtons.Keys)
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
    /// Allows the player to close the quiz and continue the game without solving it
    /// </summary>
    private void OnCloseClicked()
    {
        associatedObject = null;
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);

        answerButtons.Clear();
    }

    /// <summary>
    /// Loads the question data and shows it in the quiz screen.
    /// </summary>
    public void ShowQuiz(QuizQuestion questionToShow, InteractableObject interactedObject)
    {
        // initialize timer to answer the question
        associatedObject = interactedObject;

        uiDocument.rootVisualElement.Q<Label>("Question").text = questionToShow.question;
        uiDocument.rootVisualElement.Q<VisualElement>("AnswersContainer").Clear();

        // use toggle buttons for multiple right answers
        if (questionToShow.rightAnswers.Length > 1)
        {
            uiDocument.rootVisualElement.Q<Label>("Instruction").text = "Marque todas as respostas corretas.";
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
            uiDocument.rootVisualElement.Q<Label>("Instruction").text = "Marque apenas uma resposta.";
            foreach (var answer in questionToShow.rightAnswers)
            {
                AddRadioButton(answer, true);
            }
            foreach (var answer in questionToShow.wrongAnswers)
            {
                AddRadioButton(answer, false);
            }
        }

        ShuffleAnswers(uiDocument.rootVisualElement.Q<VisualElement>("AnswersContainer"));

        // disables the submit answer by default until an answer is selected
        uiDocument.rootVisualElement.Q<Button>("SubmitButton").SetEnabled(false);

        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.QUIZ);
    }

    /// <summary>
    /// Instantiate radio buttons for the answer options, used for questions where theres only one right answer.
    /// </summary>
    /// <param name="text">Display text for this answer.</param>
    /// <param name="desiredAnswer">The correct state to solve this quiz.</param>
    private void AddRadioButton(string text, bool desiredAnswer)
    {
        VisualElement answersList = uiDocument.rootVisualElement.Q<VisualElement>("AnswersContainer");

        RadioButton newButton = new RadioButton();
        newButton.text = text;
        newButton.value = false;
        newButton.AddToClassList("answers");
        answersList.Add(newButton);
        answerButtons.Add(newButton, desiredAnswer);
        
        newButton.RegisterCallback<ChangeEvent<bool>>(evt => OnAnswerSelected());
    }

    /// <summary>
    /// Instantiate toggle buttons for the answer options, used for questions with multiple right answer.
    /// </summary>
    /// <param name="text">Display text for this answer.</param>
    /// <param name="desiredAnswer">The correct state to solve this quiz.</param>
    private void AddToggleButton(string text, bool desiredAnswer)
    {
        VisualElement answersList = uiDocument.rootVisualElement.Q<VisualElement>("AnswersContainer");

        Toggle newButton = new Toggle();
        newButton.AddToClassList("answers");
        newButton.text = text;
        newButton.value = false;
        newButton.Children();
        answersList.Add(newButton);
        answerButtons.Add(newButton, desiredAnswer);

        newButton.RegisterCallback<ChangeEvent<bool>>(evt => OnAnswerSelected());
    }

    /// <summary>
    /// Reorders orders of the answers to show.
    /// </summary>
    /// <param name="answerContainer">The Visual Element containing the answers.</param>
    private void ShuffleAnswers(VisualElement answerContainer)
    {
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
}
