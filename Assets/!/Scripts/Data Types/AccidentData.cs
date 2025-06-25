using UnityEngine;

[CreateAssetMenu(fileName = "NewAccidentData", menuName = "Scriptable Objects/New Accident Data")]
public class AccidentData : ScriptableObject
{
    public Sprite accidentImage;
    public string accidentTitle;
    public string accidentDescription;
    public AudioClip accidentAudio;
}
