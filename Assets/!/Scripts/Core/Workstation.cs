using UnityEngine;

public class Workstation : MonoBehaviour
{
    // manages the workstation, where a worker will go to work and die (?)
    // TODO: make it derived from Patrol Point, since many funcionalities are the same

    [Header("Ref")]
    [SerializeField] private WorkerManager workerManager;
    [SerializeField] private GameObject workspot;
    [HideInInspector] public bool isOccupied = false;

    public Vector3 GetWorkspotPoint()
    {
        return workspot.transform.position;
    }
}
