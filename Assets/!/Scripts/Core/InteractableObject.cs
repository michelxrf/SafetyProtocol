using UnityEngine;

    /// <summary>
    /// Manages clickable objects in the level to be solved, like ambient hazzards.
    /// </summary>
public class InteractableObject : MonoBehaviour
{
    [SerializeField] protected WorkerManager workerManager;
    [SerializeField] protected HudManager hud;

    protected virtual void Awake()
    {
        if (workerManager == null)
            workerManager = FindFirstObjectByType<WorkerManager>();

        if (hud == null)
            hud = FindFirstObjectByType<HudManager>();
    }

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
        // TODO: update GameManager and UI

        GetComponent<InventorySystem>().ReverseEquipment();
        GetComponent<Clickable>().questionData = null;
    }

    /// <summary>
    /// The Quiz was wrongly answered.
    /// </summary>
    protected virtual void AnswereWrong()
    {

    }
}
