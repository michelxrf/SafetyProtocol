using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PatrolPoint))]
public class Workstation : MonoBehaviour
{
    // component attached to some patrol points to expand them to work as workstations

    public GameObject workerLookAt;
    [HideInInspector] public PatrolPoint assossiatedPatrolPoint;
    //public Worker.JOB_TYPE workerType;
    public string workAnimation = "Work";

    [SerializeField] private List<Worker> allowedWorkers;

    private void Awake()
    {
        assossiatedPatrolPoint = GetComponent<PatrolPoint>();
    }

    private void Start()
    {
        

        if (workerLookAt == null)
        {
            workerLookAt = GetComponentInChildren<MeshRenderer>().gameObject;
        }
    }

    /// <summary>
    /// Tests if a worker can work at this station
    /// </summary>
    /// <returns>True for if can work, false otherwise</returns>
    public bool IsWorkerAllowed(Worker worker)
    {
        return allowedWorkers.Contains(worker);
    }
}
