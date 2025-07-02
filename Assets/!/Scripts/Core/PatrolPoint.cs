using UnityEngine;

/// <summary>
/// Patrol points are the map locations that workers will move to and from on the level
/// </summary>
public class PatrolPoint : MonoBehaviour
{
    [SerializeField] private WorkerManager workerManager;
    public Worker assignedWorker;
    [HideInInspector] public bool isWorkerHere = false;

    private void Awake()
    {
        workerManager = FindFirstObjectByType<WorkerManager>();

        // hides the markers
        GetComponent<MeshRenderer>().enabled = workerManager.debugMode;
    }


    /// <summary>
    /// Assign a worker to this point
    /// </summary>
    /// <param name="worker">The worker that will be assinged</param>
    public void AssignWorker(Worker worker)
    {
        assignedWorker = worker;
    }

    /// <summary>
    /// Clears references to assigned worker
    /// </summary>
    public void FreePoint()
    {
        if (assignedWorker == null)
            return;

        assignedWorker.assignedPoint = null;
        assignedWorker = null;
    }
}
