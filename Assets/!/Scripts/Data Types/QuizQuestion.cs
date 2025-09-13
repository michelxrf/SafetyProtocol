using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Scriptable Objects/New Quiz Question")]
public class QuizQuestion : ScriptableObject
{
    [TextArea(5, 20)]
    public string question = "Lorem Ipsum?";
    public string[] rightAnswers = { "a"};
    public string[] wrongAnswers = { "b", "c", "d" };

    public string viewportTitle = "Titulo";
    public string viewportUpperText = "Descrição acima do view";
    public string viewportLowerText = "Descrição abaixo do view";

}
