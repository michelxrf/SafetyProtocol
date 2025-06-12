using UnityEngine;

public class Hazard : InteractableObject
{
    protected override void AnswereWrong()
    {
        base.AnswereWrong();
        DisableInteraction();
    }

    private void DisableInteraction()
    {
        Destroy(GetComponent<Clickable>());
    }
}
