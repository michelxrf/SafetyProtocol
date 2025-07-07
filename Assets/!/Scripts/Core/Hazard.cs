using UnityEngine;


/// <summary>
/// control specific behavior to ambiental hazards
/// </summary>
public class Hazard : InteractableObject
{
    /// <summary>
    /// It was solved correctly, score and go on with the game.
    /// </summary>
    protected override void Solve()
    {
        base.Solve();

        workerManager.solvedHazzards += 1;
        hud.UpdateScores();
        hud.Show();
        DisableInteraction();
    }

    /// <summary>
    /// Wrong answer! Just go on with the game and disables it.
    /// </summary>
    protected override void AnswereWrong()
    {
        base.AnswereWrong();
        hud.Show();
        DisableInteraction();
    }

    /// <summary>
    /// Prevents the player from interaction with it again.
    /// </summary>
    private void DisableInteraction()
    {
        Destroy(GetComponent<Clickable>());
    }
}
