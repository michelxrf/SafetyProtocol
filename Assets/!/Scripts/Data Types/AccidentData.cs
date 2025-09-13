using UnityEngine;

[CreateAssetMenu(fileName = "NewAccidentData", menuName = "Scriptable Objects/New Accident Data")]
public class AccidentData : ScriptableObject
{
    public bool dontDestroyOnRandomization = false;
    public Texture2D accidentImage;
    public string accidentTitle = "Generic Accident!";
    [TextArea(5, 15)]
    public string accidentDescription = "Lorem Ipsum Doloren";
    public AudioClip accidentAudio;
}
