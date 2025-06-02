using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    // Directs the workers on a level, sending them to random patrols, workstations and accident events

    public bool debugMode = true;
    [SerializeField] private bool generateRandomPatrolPoints;

    private List<PatrolPoint> patrolPoints = new();
    private List<Worker> workers = new();
    private List<Workstation> workstations = new();
    

    private void Awake()
    {
        GetAllWorkersInScene();
        GetAllPatrolPointsInScene();
    }
    private void GetAllWorkersInScene()
    {
        // as the name suggests, list all workers
        // TODO: experiment with workers registering themselves

        workers = (FindObjectsByType<Worker>(FindObjectsSortMode.None)).ToList<Worker>();
    }

    private void GetAllPatrolPointsInScene()
    {
        // as the name sugest, list all patrol points in scecne
        // TODO: experiment with patrol point self registering

        PatrolPoint[] manuallySetPatrolPoints = FindObjectsByType<PatrolPoint>(FindObjectsSortMode.None);
        
        if (manuallySetPatrolPoints.Length > 0)
        {
            // add all designer made patrol points

            patrolPoints.AddRange(manuallySetPatrolPoints);
        }

        if (generateRandomPatrolPoints)
        {
            // generate patrol points based on the navMesh

            patrolPoints.AddRange(GetComponent<TriangulationSampler>().GenerateRandomPatrolPoints());
        }

        if (patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points in the scene!");
        }

        if (workers.Count > patrolPoints.Count + 1)
        {
            Debug.LogWarning("Not enough patrol points for the level's worker population!");
        }
    }

    public PatrolPoint GetRandomPatrolPoint()
    {
        // returns a free patrol point, used to send workers wandering around

        List<PatrolPoint> freePatrolPoints = patrolPoints.FindAll(n => n.isFree);

        if (freePatrolPoints.Count > 0)
        {
            return freePatrolPoints[Random.Range(0, freePatrolPoints.Count)];
        }
        else
        {
            Debug.LogWarning("No free patrol points!");
            return null;
        }

    }
}

