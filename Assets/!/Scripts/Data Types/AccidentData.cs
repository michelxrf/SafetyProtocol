using UnityEngine;

[CreateAssetMenu(fileName = "NewAccidentData", menuName = "Scriptable Objects/New Accident Data")]
public class AccidentData : ScriptableObject
{
    public Texture2D accidentImage;
    public string accidentTitle = "Generic Accident!";
    public string accidentDescription = "Lorem Ipsum Doloren";
    public AudioClip accidentAudio;
}
