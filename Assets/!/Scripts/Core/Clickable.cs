using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Clickable : MonoBehaviour
{
    // makes the object reacts to a click
    // IDEAS: make the object play a sfx once clicked

    [SerializeField] private QuizQuestion questionData;
    private UiManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UiManager>();
    }

    public void OnClick()
    {
        if ( questionData != null )
        {
            uiManager.ShowQuiz(questionData, GetComponent<InteractableObject>());
            //Invoke(nameof(Test), 0.1f);
        }
    }

    private void Test()
    {
        uiManager.ShowQuiz(questionData, GetComponent<InteractableObject>());
    }
}
