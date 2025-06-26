using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

/// <summary>
/// Directs the workers on a level, sending them to random patrols, workstations and accident events.
/// </summary>
public class WorkerManager : MonoBehaviour
{
    [SerializeField] CameraController playerCamera;

    [Header("Settings")]
    public bool debugMode = true;
    [SerializeField] private bool generateRandomPatrolPoints;
    private bool isGamePaused = false;

    [Header("Worker Behavior")]
    [Range(0f, 1f)]
    [SerializeField] private float idleChance;

    [Header("Accidents")]
    [SerializeField] private ACCIDENTORDER accidentOrder = ACCIDENTORDER.RANDOM;
    public float accidentCountdownTime = 5f;
    [SerializeField] public List<AccidentEvent> accidentEventsList;
    [HideInInspector] public float accidentRemainingTime;
    [HideInInspector] public bool isCountingDown = false;
    private Worker workerInAccidentEvent;
    private AccidentData currentAccidentData;
    public int solvedAccidents = 0;
    public int totalAccidents = 0;
    private enum ACCIDENTORDER { RANDOM, SEQUENCE };

    private List<PatrolPoint> patrolPoints = new();
    [HideInInspector] public List<Worker> workers = new();
    private List<Workstation> workstations = new();


    private void Awake()
    {
        GetAllWorkersInScene();
        GetAllPatrolPointsInScene();

        totalAccidents = accidentEventsList.Count;

        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<CameraController>();
        }
    }

    /// <summary>
    /// Finds all Workers in the level and saves them in a list.
    /// </summary>
    private void GetAllWorkersInScene()
    {
        // as the name suggests, list all workers
        // TODO: experiment with workers registering themselves

        workers = (FindObjectsByType<Worker>(FindObjectsSortMode.None)).ToList<Worker>();
    }

    /// <summary>
    /// Collects all Patrol Points present in the level in a list.
    /// </summary>
    private void GetAllPatrolPointsInScene()
    {
        PatrolPoint[] manuallySetPatrolPoints = FindObjectsByType<PatrolPoint>(FindObjectsSortMode.None);

        if (manuallySetPatrolPoints.Length > 0)
        {
            // adds all designer made patrol points

            foreach (PatrolPoint patrolPoint in manuallySetPatrolPoints)
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

    private void Start()
    {
        InitAllWorkersMovement();
        CallNextAccident();
    }

    private void Update()
    {
        CountdownToAccident();
    }

    /// <summary>
    /// Called in the level start to order all workers to move around randomly.
    /// </summary>
    private void InitAllWorkersMovement()
    {
        foreach(Worker worker in workers)
        {
            worker.MoveToRandomPoint();
        }
    }

    /// <summary>
    /// Activate the next accident on the list
    /// </summary>
    public void CallNextAccident()
    {
        if (!(accidentEventsList.Count > 0))
        {
            Debug.LogError("Accidents list is empty. Level cleared?");
            Debug.Log($"{solvedAccidents}/{totalAccidents} accidents solved");
            return;
        }

        AccidentEvent nextAccident = new AccidentEvent();
        switch (accidentOrder)
        {
            case ACCIDENTORDER.RANDOM:
                int randIndex = Random.Range(0, accidentEventsList.Count);
                nextAccident = accidentEventsList[randIndex];
                accidentEventsList.RemoveAt(randIndex);
                break;

            case ACCIDENTORDER.SEQUENCE:
                nextAccident = accidentEventsList[0];
                accidentEventsList.RemoveAt(0);
                break;

            default:
                break;
        }

        currentAccidentData = nextAccident.accidentData;
        nextAccident.worker.SetQuizData(nextAccident.quizQuestion);
        SendWorkerToAccident(nextAccident.worker, nextAccident.patrolPoint);
    }

    /// <summary>
    /// returns the Patrol Point of a random free workstation
    /// </summary>
    public PatrolPoint GetRandomWorkstation()
    {
        List<Workstation> freeWorkstation = workstations.FindAll(n => n.assossiatedPatrolPoint == null);

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

    /// <summary>
    /// Returns a random free patrol point, either a workstation or simple patrol point.
    /// Worsktation chance is defined by idleChance variable.
    /// </summary>
    public PatrolPoint GetAnyRandomPoint()
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

    /// <summary>
    /// Returns a free simple patrol point.
    /// </summary>
    public PatrolPoint GetRandomPatrolPoint()
    {
        // returns a free patrol point, used to send workers wandering around

        List<PatrolPoint> freePatrolPoints = patrolPoints.FindAll(n => n.assignedWorker == null);

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

    /// <summary>
    /// Order the worker to go to the accident spot and prepares them to respond when arrive there
    /// </summary>
    /// <param name="worker">The worker who will be ordered</param>
    /// <param name="accidentLocation">The patrol point where the accident will happen</param>
    public void SendWorkerToAccident(Worker worker, PatrolPoint accidentLocation)
    {
        // frees the assossiated point from other worker
        if ((accidentLocation.assignedWorker != null) && (accidentLocation.assignedWorker != worker))
        {
            accidentLocation.assignedWorker.MoveToRandomPoint();
        }

        worker.isAccidentTarget = true;
        workerInAccidentEvent = worker;
        worker.MoveToPoint(accidentLocation);
    }

    /// <summary>
    /// Shows the UI alert with the countdown to solution
    /// </summary>
    public void StartAccidentCountdown()
    {
        isCountingDown = true;
        accidentRemainingTime = accidentCountdownTime;
    }

    /// <summary>
    /// Decreases countdown to accident event
    /// </summary>
    private void CountdownToAccident()
    {
        if (!isCountingDown || isGamePaused)
            return;

        accidentRemainingTime -= Time.deltaTime;
        Debug.Log($"time remaining to solve accident: {accidentRemainingTime.ToString("#0.0")}");

        if (accidentRemainingTime < 0)
        {
            isCountingDown = false;
            workerInAccidentEvent.AccidentTimeOver();
            workerInAccidentEvent = null;
            CallNextAccident();
        }
    }

    /// <summary>
    /// Stops any time related process like accident countdown, prevent camera movement and freezes animations.
    /// </summary>
    public void PauseGame()
    {
        isGamePaused = true;
        playerCamera.isMovementAllowed = false;

        foreach(Worker worker in workers)
        {
            worker.FreezeAnimation();
        }
    }

    /// <summary>
    /// Resumes time related processes like accident countdown, prevent camera movement and freezes animations.
    /// </summary>
    public void UnpauseGame()
    {
        isGamePaused = false;
        playerCamera.isMovementAllowed = true;

        foreach (Worker worker in workers)
        {
            worker.ResumeAnimation();
        }
    }
}


/// <summary>
/// Used to allow the level designer to assossiate accident objects to workers and patrol points or workstations
/// </summary>
[System.Serializable]
public class AccidentEvent
{
    public AccidentData accidentData;
    public Worker worker;
    public PatrolPoint patrolPoint;
    public QuizQuestion quizQuestion;
}

