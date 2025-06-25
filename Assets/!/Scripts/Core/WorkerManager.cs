using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    // Directs the workers on a level, sending them to random patrols, workstations and accident events

    [Header("Settings")]
    public bool debugMode = true;
    [SerializeField] private bool generateRandomPatrolPoints;

    [Header("Worker Behavior")]
    [Range(0f, 1f)]
    [SerializeField] private float idleChance;

    [Header("Accidents")]
    [SerializeField] public List<AccidentEvent> accidentEvents;

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
            
            foreach(PatrolPoint patrolPoint in manuallySetPatrolPoints)
            {
                if (patrolPoint.GetComponent<Workstation>() != null)
                {
                    workstations.Add(patrolPoint.GetComponent<Workstation>());
                }
                else
                {
                    patrolPoints.Add(patrolPoint);
                }
            }
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

        Debug.Log($"Scene has {patrolPoints.Count} patrol points, and {workstations.Count} workstations.");
    }
    public PatrolPoint GetRandomWorkstation()
    {
        // return the Patrol Point of a random workstation

        List<Workstation> freeWorkstation = workstations.FindAll(n => n.assossiatedPatrolPoint.isFree);

        if (freeWorkstation.Count > 0)
        {
            List<PatrolPoint> assossiatedPatrols = new List<PatrolPoint>();

            foreach (Workstation workstation in freeWorkstation)
            {
                assossiatedPatrols.Add(workstation.assossiatedPatrolPoint);
            }

            return assossiatedPatrols[Random.Range(0, freeWorkstation.Count)];
        }

        else
        {
            Debug.LogWarning("No freeworkstation");
            return null;
        }
    }

    public PatrolPoint GetRandomPoint()
    {
        if (idleChance <= Random.Range(0f, 1f))
        {
            return GetRandomPatrolPoint();
        }
        else
        {
            return GetRandomWorkstation();
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

[System.Serializable]
public class AccidentEvent
{
    public AccidentData accidentData;
    public Worker worker;
    public Workstation workstation;
    public int score;
}

