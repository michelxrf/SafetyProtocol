using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// Controls the workers in the game level.
/// </summary>
public class Worker : InteractableObject
{
    [Header("References")]
    private AgentDestinationReachedNotifier destinationReachedNotifier;
    private Animator animator;
    private AccidentScreen accidentScreen;

    [Header("Navigation")]
    private NavMeshAgent navMeshAgent;
    private float navMeshAgentSpeed;
    [SerializeField] private float minIdleTime = 5f;
    [SerializeField] private float maxIdleTime = 20f;

    [Header("Work")]
    [SerializeField] private float minWorkTime = 5f;
    [SerializeField] private float maxWorkTime = 20f;
    [SerializeField] public JOB_TYPE workerType;
    [HideInInspector] public STATE currentState;
    public enum STATE { IDLING, MOVING, WORKING };
    public enum JOB_TYPE { CONSTRUCTION, ELECTRICAL, HEIGHT };
    [HideInInspector] public PatrolPoint assignedPoint;
    public bool isAccidentTarget = false;


    protected override void Awake()
    {
        base.Awake();

        // disables worker quiz till it gets called to accident
        GetComponent<Clickable>().isEnabled = false;
        GetComponent<Clickable>().questionData = null;

        // gets references
        accidentScreen = FindFirstObjectByType<AccidentScreen>();
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        destinationReachedNotifier = GetComponent<AgentDestinationReachedNotifier>();

        // saves navMeshAgent speed for pausing and resuming worker movement
        navMeshAgentSpeed = navMeshAgent.speed;
    }

    /// <summary>
    /// controls what the worker does when they arrive at their destination
    /// if simply roaming just go into idle
    /// if is the accident target, start accident countdown
    /// </summary>
    private void ReachDestination()
    {
        // makes the worker clickable and starts countdown to solution
        animator.SetBool("isWalking", false);

        bool isDestinationAWorkstation = assignedPoint.GetComponent<Workstation>() != null ? true : false;
        if (isDestinationAWorkstation)
        {
            // if the spot the worker arrived is a workstation, do the following

            currentState = STATE.WORKING;
            animator.SetBool("isWorking", true);

            RotateTo(assignedPoint.GetComponent<Workstation>().workerLookAt);

            if (!isAccidentTarget)
                StartCoroutine(WaitForSeconds(minWorkTime, maxWorkTime));
        }
        else
        {
            // if it's not a workstation then is just a normal patrol point

            currentState = STATE.IDLING;
            animator.SetBool("isWorking", false);

            if (!isAccidentTarget)
                StartCoroutine(WaitForSeconds(minIdleTime, maxIdleTime));
        }

        if (isAccidentTarget)
        {
            workerManager.StartAccidentCountdown();
            GetComponent<Clickable>().isEnabled = true;
            isAccidentTarget = false;
        }
    }

    /// <summary>
    /// rotates the worker so it looks at the workstation they just arrived.
    /// </summary>
    /// <param name="target">The game object that the worker will look at.</param>
    private void RotateTo(GameObject target)
    {
        navMeshAgent.updateRotation = false;

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }

    /// <summary>
    /// Wait for a random range amount of time, used for idling workers.
    /// </summary>
    /// <param name="minSeconds">Min time range.</param>
    /// <param name="maxSeconds">Max time range.</param>
    /// <returns></returns>
    private IEnumerator WaitForSeconds(float minSeconds, float maxSeconds)
    {
        Debug.Log($"{gameObject.name} is on wait coroutine");
        yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
        MoveToRandomPoint();
    }

    private void OnEnable()
    {
        // register the action call so it triggers when destination is reached
        destinationReachedNotifier.OnDestinationReached += ReachDestination;
    }

    private void OnDisable()
    {
        // de-register the action call so it won't crash by calling a disabled script
        destinationReachedNotifier.OnDestinationReached -= ReachDestination;
    }
    /// <summary>
    /// sends the worker to a specific patrol point on the level
    /// </summary>
    /// <param name="patrolPoint">The Patrol Point the worker will move to.</param>
    public void MoveToPoint(PatrolPoint patrolPoint)
    {
        StopAllCoroutines();

        FreeAssignedPoint();
        assignedPoint = patrolPoint;
        assignedPoint.AssignWorker(this);

        animator.SetBool("isWalking", true);
        animator.SetBool("isWorking", false);

        navMeshAgent.updateRotation = true;
        navMeshAgent.SetDestination(patrolPoint.transform.position);
    }

    /// <summary>
    /// clears references to the current patrol point
    /// </summary>
    public void FreeAssignedPoint()
    {
        if (assignedPoint == null)
            return;

        assignedPoint.assignedWorker = null;
        assignedPoint = null;
    }


    /// <summary>
    /// sends the worker to a random patrol point
    /// </summary>
    public void MoveToRandomPoint()
    {
        PatrolPoint nextTarget = workerManager.GetAnyRandomPoint();
                
        if (nextTarget != null)
        {
            MoveToPoint(nextTarget);
        }
        else
        {
            MoveToRandomPoint();
        }
    }

    /// <summary>
    /// Deletes the worker, used for failed accidents
    /// </summary>
    private void KillWorker()
    {
        assignedPoint.FreePoint();
        workerManager.workers.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when the accident timer finishes, meaning the player didn't find it in time
    /// </summary>
    public void AccidentTimeOver()
    {
        AnswereWrong();
    }

    /// <summary>
    /// Sets the worker quiz data, used for the accident solution.
    /// </summary>
    /// <param name="newQuestionData">The Scriptable Object containing the question that will show on the quiz.</param>
    public void SetQuizData(QuizQuestion newQuestionData)
    {
        GetComponent<Clickable>().questionData = newQuestionData;
    }

    /// <summary>
    /// Called when quiz is answered correctly
    /// </summary>
    protected override void Solve()
    {
        base.Solve();

        workerManager.solvedAccidents += 1;
        workerManager.isCountingDown = false;
        workerManager.accidentActive = false;
        hud.HideAlert();
        hud.UpdateScores();
        hud.Show();
        workerManager.CallNextAccident();
        StartCoroutine(WaitForSeconds(minWorkTime, maxWorkTime));
    }

    /// <summary>
    /// Treats a wrong call to the quiz.
    /// </summary>
    protected override void AnswereWrong()
    {
        base.AnswereWrong();
        workerManager.accidentActive = false;
        workerManager.isCountingDown = false;
        hud.HideAlert();

        accidentScreen.Show(workerManager.currentAccidentData);

        workerManager.CallNextAccident();
        KillWorker();
    }


    /// <summary>
    /// Makes worker animations
    /// </summary>
    public void FreezeAnimation()
    {
        animator.speed = 0f;
        navMeshAgent.speed = 0f;
    }

    /// <summary>
    /// Resumes freezed animation
    /// </summary>
    public void ResumeAnimation()
    {
        animator.speed = 1f;
        navMeshAgent.speed = navMeshAgentSpeed;
    }
}
