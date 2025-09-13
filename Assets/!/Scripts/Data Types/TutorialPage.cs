using UnityEngine;

/// <summary>
/// Contains the text displayed at the tutorial screen
/// </summary>
[CreateAssetMenu(fileName = "newTutorialPage", menuName = "Scriptable Objects/New Tutorial Page")]
public class TutorialPage : ScriptableObject
{
    public string title;
    [TextArea(5,15)]
    public string body;
}
