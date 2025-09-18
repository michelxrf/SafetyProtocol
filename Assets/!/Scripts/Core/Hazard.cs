using UnityEngine;


/// <summary>
/// control specific behavior to ambiental hazards
/// </summary>
public class Hazard : InteractableObject
{
    [SerializeField] AudioClip failSFX;
    [SerializeField] Canvas tip;

    private void Awake()
    {
        viewportCamera = GetComponentInChildren<Camera>();
        viewportCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
        WorkerManager.Instance.hazards.Add(this);
        WorkerManager.Instance.totalHazzards++;
        WorkerManager.Instance.ForceHudUpdate();

        tip.enabled = SettingsKeeper.Instance.dificultyLevel == 0;
    }

    /// <summary>
    /// It was solved correctly, score and go on with the game.
    /// </summary>
    protected override void Solve()
    {
        base.Solve();

        GetComponent<InventorySystem>().ReverseEquipment();
        WorkerManager.Instance.HazzardSolved();
        DisableInteraction();
    }

    /// <summary>
    /// Wrong answer! Just go on with the game and disables it.
    /// </summary>
    public override void AnswereWrong()
    {
        base.AnswereWrong();
        AudioManager.Instance.PlaySFX(failSFX, transform);
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
        DisableInteraction();
    }

    /// <summary>
    /// Prevents the player from interaction with it again.
    /// </summary>
    public void DisableInteraction()
    {
        Destroy(GetComponent<Clickable>());
        tip.enabled = false;
    }
}
