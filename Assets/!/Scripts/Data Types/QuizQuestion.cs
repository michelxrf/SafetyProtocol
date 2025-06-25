using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Scriptable Objects/New Quiz Question")]
public class QuizQuestion : ScriptableObject
{
    public string question = "Lorem Ipsum?";
    public string[] rightAnswers = { "a"};
    public string[] wrongAnswers = { "b", "c", "d" };
}
