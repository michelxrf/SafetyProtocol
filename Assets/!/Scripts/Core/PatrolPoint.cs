using UnityEngine;

/// <summary>
/// Patrol points are the map locations that workers will move to and from on the level
/// </summary>
public class PatrolPoint : MonoBehaviour
{
    [HideInInspector] public Worker assignedWorker;
    [HideInInspector] public bool isWorkerHere = false;

    private void Start()
    {
        // hides the markers
        GetComponent<MeshRenderer>().enabled = WorkerManager.Instance.debugMode;

        Workstation workstation = GetComponent<Workstation>();
        if (workstation != null )
        {
            WorkerManager.Instance.workstations.Add(workstation);
        }
        else
        {
            WorkerManager.Instance.patrolPoints.Add(this);
        }
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
