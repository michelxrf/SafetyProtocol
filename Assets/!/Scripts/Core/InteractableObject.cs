using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    public void OnQuizEnd(bool answeredCorrectly)
    {
        // callback from the quiz being answered

        if (answeredCorrectly)
        {
            Solve();
        }
        else
        {
            AnswereWrong();
        }
    }
    protected virtual void Solve()
    {
        // considers the worker as solved
        // TODO: update GameManager and UI

        GetComponent<InventorySystem>().ReverseEquipment();
        Destroy(GetComponent<Clickable>());
    }

    protected virtual void AnswereWrong()
    {

    }
}
