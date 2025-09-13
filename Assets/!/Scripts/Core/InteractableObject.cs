using UnityEngine;

    /// <summary>
    /// Manages clickable objects in the level to be solved, like ambient hazzards.
    /// </summary>
public class InteractableObject : MonoBehaviour
{
    [HideInInspector] public Camera viewportCamera;
    [SerializeField] AudioClip successSFX;

    /// <summary>
    /// callback from quiz being answered.
    /// </summary>
    /// <param name="answeredCorrectly"></param>
    public void OnQuizEnd(bool answeredCorrectly)
    {
        if (answeredCorrectly)
        {
            Solve();
        }
        else
        {
            AnswereWrong();
        }
        Destroy(GetComponent<Clickable>());
    }

    /// <summary>
    /// The quiz was correctly answered.
    /// </summary>
    protected virtual void Solve()
    {
        AudioManager.Instance.PlaySFX(successSFX, transform);
        GetComponent<Clickable>().questionData = null;
    }

    /// <summary>
    /// The Quiz was wrongly answered.
    /// </summary>
    public virtual void AnswereWrong()
    {

    }
}
