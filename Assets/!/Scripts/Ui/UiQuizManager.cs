using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the pop up screen for the quiz
/// </summary>
public class UiQuizManager : MonoBehaviour
{
    [SerializeField] private HudManager hud;
    [SerializeField] private OnScreenControls onScreenControls;
    [SerializeField] private WorkerManager workerManager;
    [SerializeField] ClickHandler clickHandler;
    private InteractableObject associatedObject;
    private UIDocument quizUi;

    // list to keep track of player answering
    private Dictionary<VisualElement, bool> answerButtons = new();

    private void Awake()
    {
        workerManager = FindFirstObjectByType<WorkerManager>();

        if (clickHandler == null)
        {
            clickHandler = FindAnyObjectByType<ClickHandler>();
        }
        quizUi = GetComponent<UIDocument>();
        quizUi.rootVisualElement.style.display = DisplayStyle.None;

        quizUi.rootVisualElement.Q<Button>("SubmitButton").clicked += OnSubmitClick;

        if (onScreenControls ==  null)
            onScreenControls = FindFirstObjectByType<OnScreenControls>();

        if (hud == null)
            hud = FindFirstObjectByType<HudManager>();
    }

    /// <summary>
    /// Prevents the submit button from being clicked if not a single answer has been selected.
    /// </summary>
    /// <returns>True if more than zero answers are selected, false otherwise.</returns>
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
        HideQuiz();
        hud.Show();

        hud.HideAlert();
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
        hud.Hide();
        onScreenControls.Hide();
        workerManager.isCountingDown = false;

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
        workerManager.PauseGame();
    }

    /// <summary>
    /// Instantiate radio buttons for the answer options, used for questions where theres only one right answer.
    /// </summary>
    /// <param name="text">Display text for this answer.</param>
    /// <param name="desiredAnswer">The correct state to solve this quiz.</param>
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

    /// <summary>
    /// Instantiate toggle buttons for the answer options, used for questions with multiple right answer.
    /// </summary>
    /// <param name="text">Display text for this answer.</param>
    /// <param name="desiredAnswer">The correct state to solve this quiz.</param>
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


    /// <summary>
    /// Makes the quiz screen disapper.
    /// </summary>
    public void HideQuiz()
    {
        clickHandler.canClick = true;
        quizUi.rootVisualElement.style.display = DisplayStyle.None;
        workerManager.UnpauseGame();
        hud.Show();
        onScreenControls.Show();
    }
}
