using UnityEngine;

public class Hazard : InteractableObject
{
    // control specific behavior to ambiental hazards

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
