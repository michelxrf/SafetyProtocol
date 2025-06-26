using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// handles switching of UI screens as game changes state
/// </summary>
public class UiManager : MonoBehaviour
{
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
