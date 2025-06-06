using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Clickable : MonoBehaviour
{
    // makes the object reacts to a click
    // IDEAS: make the object play a sfx once clicked
    // TODO: add a quiz dataset
    // TODO: if the clicable have a quiz, call it

    [SerializeField] private QuizQuestion questionData;
    private UiManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UiManager>();
    }

    public void OnClick()
    {
        Debug.Log($"{gameObject.name} just got clicked.");

        if ( questionData != null )
        {
            uiManager.ShowQuiz(questionData);
        }
    }
}
