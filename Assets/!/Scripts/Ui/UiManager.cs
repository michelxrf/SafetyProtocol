using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    // handles switching UI elements as game changes state

    [SerializeField] private UIDocument pauseMenu;
    [SerializeField] private UiQuizManager quiz;
    [SerializeField] private UIDocument hud;

    private void Awake()
    {
        //pauseMenu.enabled = false;
        hud.transform.gameObject.SetActive(true);
    }

    public void ShowHud()
    {
        quiz.HideQuiz();
        hud.transform.gameObject.SetActive(true);
    }

    public void ShowQuiz(QuizQuestion questionToShow, InteractableObject interactedObject)
    {
        hud.transform.gameObject.SetActive(false);
        quiz.ShowQuiz(questionToShow, interactedObject);
    }
}
