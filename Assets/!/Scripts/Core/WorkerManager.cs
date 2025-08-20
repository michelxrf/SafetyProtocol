using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

/// <summary>
/// Directs the workers on a level, sending them to random patrols, workstations and accident events.
/// </summary>
public class WorkerManager : MonoBehaviour
{
    // Singleton vars
    public static WorkerManager Instance { get; private set; }

    CameraController playerCamera;
    HudManager hudManager;
    AccidentScreen accidentScreen;
    GameEndScreen gameEndScreen;

    [Header("Settings")]
    public bool debugMode = true;
    [SerializeField] private bool generateRandomPatrolPoints;
    private bool isGamePaused = false;
    [SerializeField] int solveAccidentScore = 100;
    [SerializeField] int solveHazzardScore = 50;
    [SerializeField] int perSecondScore = 3;

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
    [HideInInspector] public AccidentData currentAccidentData;
    [HideInInspector] public bool accidentActive = false;
    [HideInInspector] public int solvedAccidents = 0;
    [HideInInspector] public int totalAccidents = 0;
    [HideInInspector] public int solvedHazzards = 0;
    [HideInInspector] public int totalHazzards = 0;
    private enum ACCIDENTORDER { RANDOM, SEQUENCE };
    private int score = 0;

    [HideInInspector] public List<PatrolPoint> patrolPoints = new();
    [HideInInspector] public List<Worker> workers = new();
    [HideInInspector] public List<Workstation> workstations = new();

    [HideInInspector] public float gameTime = 0f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            totalHazzards = FindObjectsByType<Hazard>(FindObjectsSortMode.None).Length;
            totalAccidents = accidentEventsList.Count;

            if (playerCamera == null)
                playerCamera = FindFirstObjectByType<CameraController>();

            if (hudManager == null)
                hudManager = FindFirstObjectByType<HudManager>();

            if (accidentScreen == null)
                accidentScreen = FindFirstObjectByType<AccidentScreen>();

            if (gameEndScreen == null)
                gameEndScreen = FindFirstObjectByType<GameEndScreen>();

            // generate patrol points based on the navMesh
            if (generateRandomPatrolPoints)
            {
                patrolPoints.AddRange(GetComponent<TriangulationSampler>().GenerateRandomPatrolPoints());
            }
        }
    }

    /// <summary>
    /// Called when player fails to solve the accident, it should display the accident screen
    /// </summary>
    public void AccidentHappened()
    {
        accidentScreen.Show(currentAccidentData);
        ClearAccident();
    }

    /// <summary>
    /// Disables current accident without scoring, used for failed attempts
    /// </summary>
    public void ClearAccident()
    {
        accidentActive = false;
        isCountingDown = false;
        workerInAccidentEvent = null;
    }
    private void Start()
    {
        InitAllWorkersMovement();
        CallNextAccident();
    }

    private void Update()
    {
        CountdownToAccident();
        CountGametime();
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
            currentAccidentData = null;

            Debug.LogWarning("Accidents list is empty. Level cleared?");
            ShowEndGame();
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

    public void ShowEndGame()
    {
        gameEndScreen.Show(score, gameTime, solvedAccidents, totalAccidents, solvedHazzards, totalHazzards);
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAMEEND);
        Debug.Log($"A: {solvedAccidents}/{totalAccidents}, H:{solvedHazzards}/{totalHazzards}");

        Debug.Log($"{LeaderboardManager.Instance.playerScore} - {score}");
        if (LeaderboardManager.Instance.playerScore < score)
        {
            LeaderboardManager.Instance.SubmitCurrentScore(score);
        }
        else
        {
            LeaderboardManager.Instance.GetTop10Scores();
        }
    }

    /// <summary>
    /// returns the Patrol Point of a random free workstation
    /// </summary>
    public PatrolPoint GetRandomWorkstation(Worker.JOB_TYPE jobType)
    {
        // filter non empty workstations
        List<Workstation> freeWorkstations = workstations.FindAll(n => n.assossiatedPatrolPoint.assignedWorker == null);

        // filter workstations of differnet job type
        List<Workstation> sameTagWorkstations = freeWorkstations.FindAll(n => n.workerType == jobType);

        if (sameTagWorkstations.Count > 0)
        {
            List<PatrolPoint> assossiatedPatrols = new List<PatrolPoint>();

            foreach (Workstation workstation in sameTagWorkstations)
            {
                assossiatedPatrols.Add(workstation.assossiatedPatrolPoint);
            }

            return assossiatedPatrols[Random.Range(0, sameTagWorkstations.Count)];
        }

        else
        {
            Debug.LogWarning("no free workstation found. Too few workstations for this worker population?");
            return null;
        }
    }

    /// <summary>
    /// Returns a random free patrol point, either a workstation or simple patrol point.
    /// Worsktation chance is defined by idleChance variable.
    /// </summary>
    public PatrolPoint GetRandomPoint(Worker.JOB_TYPE jobType)
    {
        if (Random.Range(0f, 1f) <= idleChance)
        {
            return GetRandomPatrolPoint();
        }
        else
        {
            return GetRandomWorkstation(jobType);
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
        accidentActive = true;
        isCountingDown = true;
        accidentRemainingTime = accidentCountdownTime;
    }

    /// <summary>
    /// Count total game time
    /// </summary>
    private void CountGametime()
    {
        if (isGamePaused)
            return;

        gameTime += Time.deltaTime;
    }

    /// <summary>
    /// Decreases countdown to accident event
    /// </summary>
    private void CountdownToAccident()
    {
        if (!isCountingDown || isGamePaused)
            return;

        accidentRemainingTime -= Time.deltaTime;

        if (accidentRemainingTime < 0)
        {
            AccidentHappened();
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

    /// <summary>
    /// Increment score and update hud
    /// </summary>
    public void HazzardSolved()
    {
        solvedHazzards += 1;
        score += solveHazzardScore;
        hudManager.UpdateScores();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);
    }

    /// <summary>
    /// Increment score and update hud
    /// </summary>
    public void AccidentSolved()
    {
        solvedAccidents += 1;
        score += solveAccidentScore + Mathf.FloorToInt(accidentRemainingTime) * perSecondScore;
        hudManager.UpdateScores();
        ScreenSelector.Instance.SwitchScreen(ScreenSelector.SCREENMODE.GAME);

        ClearAccident();
        CallNextAccident();
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

