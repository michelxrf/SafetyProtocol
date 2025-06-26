using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Clickable : MonoBehaviour
{
    // makes the object reacts to a click
    // IDEAS: make the object play a sfx once clicked

    public QuizQuestion questionData;
    private UiManager uiManager;
    [HideInInspector] public bool isEnabled = true;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UiManager>();
    }

    public void OnClick()
    {
        if (!isEnabled)
            return;

        if (questionData != null)
        {
            uiManager.ShowQuiz(questionData, GetComponent<InteractableObject>());
        }
    }
}
