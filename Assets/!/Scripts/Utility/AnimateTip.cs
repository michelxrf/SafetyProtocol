using UnityEngine;

public class AnimateTip : MonoBehaviour
{
    [SerializeField] Canvas tip;
    [SerializeField] float magnitude;
    private void Update()
    {
        tip.scaleFactor = Mathf.Sin( Time.realtimeSinceStartup ) * magnitude/2f + magnitude;
    }
}
