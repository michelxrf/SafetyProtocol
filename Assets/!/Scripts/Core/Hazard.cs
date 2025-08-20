using UnityEngine;


/// <summary>
/// control specific behavior to ambiental hazards
/// </summary>
public class Hazard : InteractableObject
{
    private void Awake()
    {
        viewportCamera = GetComponentInChildren<Camera>();
        viewportCamera.gameObject.SetActive(false);
    }

    /// <summary>
    /// It was solved correctly, score and go on with the game.
    /// </summary>
    protected override void Solve()
    {
        base.Solve();

        WorkerManager.Instance.HazzardSolved();
        DisableInteraction();
    }

    /// <summary>
    /// Wrong answer! Just go on with the game and disables it.
    /// </summary>
    protected override void AnswereWrong()
    {
        base.AnswereWrong();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
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
