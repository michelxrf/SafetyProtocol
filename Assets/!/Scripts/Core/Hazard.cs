using UnityEngine;


/// <summary>
/// control specific behavior to ambiental hazards
/// </summary>
public class Hazard : InteractableObject
{
    protected override void Solve()
    {
        base.Solve();

        workerManager.solvedHazzards += 1;
        hud.UpdateScores();
    }

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
