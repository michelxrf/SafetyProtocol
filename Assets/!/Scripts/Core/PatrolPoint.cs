using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    // Patrol points are the map locations that workers will move to and from on the level

    [SerializeField] private WorkerManager workerManager;
    [HideInInspector] public Worker assignedWorker;
    [HideInInspector] public bool isFree = true;
    [HideInInspector] public bool isWorkerHere = false;

    private void Awake()
    {
        workerManager = FindFirstObjectByType<WorkerManager>();

        // hides the markers
        GetComponent<MeshRenderer>().enabled = workerManager.debugMode;
    }

    public void AssignWorker(Worker worker)
    {
        // used together with FreePoint to limit one worker per patrol point at a time
        isFree = false;
        assignedWorker = worker;
    }

    public void FreePoint()
    {
        // used together with AssignWorker to limit one worker per patrol point at a time
        isFree = true;
        assignedWorker = null;
    }
}
