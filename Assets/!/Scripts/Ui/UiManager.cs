using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    // handles switching UI elements as game changes state
    // TODO move quiz and inventory screen manager into their own scrpits and use this one as an interface for them

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

    public void ShowQuiz(QuizQuestion questionToShow)
    {
        hud.transform.gameObject.SetActive(false);
        quiz.ShowQuiz(questionToShow);
    }
}
