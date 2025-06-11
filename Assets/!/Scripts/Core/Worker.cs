using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Worker : MonoBehaviour
{
    // controls the worker

    [Header("References")]
    [SerializeField] private WorkerManager workerManager;
    private AgentDestinationReachedNotifier destinationReachedNotifier;
    private Animator animator;

    [Header("Navigation")]
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float minIdleTime = 5f;
    [SerializeField] private float maxIdleTime = 20f;

    [Header("Work")]
    [SerializeField] private float minWorkTime = 5f;
    [SerializeField] private float maxWorkTime = 20f;
    [SerializeField] public JOB_TYPE workerType;
    [HideInInspector] public STATE currentState;
    public enum STATE { IDLING, MOVING, WORKING };
    public enum JOB_TYPE { CONSTRUCTION, ELECTRICAL, HEIGHT };
    private PatrolPoint assignedPoint;
    public bool hasAccident = false;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        destinationReachedNotifier = GetComponent<AgentDestinationReachedNotifier>();
    }

    private void Start()
    {
        MoveToRandomPoint();
    }

    private void ReachDestination()
    {
        // controls what the worker does based on where they arrived

        if (hasAccident /*TODO: and is at accident spot*/)
        {
            // TODO: special case where the accident event is activated and this worker is sent to it

            currentState = STATE.WORKING;
            animator.SetBool("isWorking", true);
            return;
        }

        bool isDestinationAWorkstation = assignedPoint.GetComponent<Workstation>() != null ? true : false;
        if (isDestinationAWorkstation)
        {
            // if the spot the worker arrived is a workstation, do the following

            currentState = STATE.WORKING;
            animator.SetBool("isWorking", true);

            RotateTo(assignedPoint.GetComponent<Workstation>().workerLookAt);

            StartCoroutine(WaitForSeconds(minWorkTime, maxWorkTime));
        }
        else
        {
            // if it's not a workstation then is just a normal patrol point

            currentState = STATE.IDLING;
            animator.SetBool("isWorking", false);
            StartCoroutine(WaitForSeconds(minIdleTime, maxIdleTime));
        }
    }

    private void RotateTo(GameObject target)
    {
        // rotates the worker so it looks at the workstation

        navMeshAgent.updateRotation = false;

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }

    private IEnumerator WaitForSeconds(float minSeconds, float maxSeconds)
    {
        // wait for a range of time

        animator.SetBool("isWalking", false);
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

    public void MoveToPoint(PatrolPoint patrolPoint)
    {
        // sends the worker to a patrol point on the level

        FreeAssignedPoint();
        assignedPoint = patrolPoint;
        assignedPoint.AssignWorker(this);

        animator.SetBool("isWalking", true);
        animator.SetBool("isWorking", false);

        navMeshAgent.updateRotation = true;
        navMeshAgent.SetDestination(patrolPoint.transform.position);
    }

    private void FreeAssignedPoint()
    {
        // clears references to the current patrol point

        if (assignedPoint == null)
            return;

        assignedPoint.FreePoint();
        assignedPoint = null;
    }

    public void MoveToRandomPoint()
    {
        // sends the worker to a random patrol point

        PatrolPoint nextTarget = workerManager.GetRandomPoint();
                
        if (nextTarget != null)
        {
            MoveToPoint(nextTarget);
        }
        else
        {
            MoveToRandomPoint();
        }
    }

    private void KillWorker()
    {
        // as the name sugests

        assignedPoint.FreePoint();
        Destroy(gameObject);
    }

    private void Solve()
    {
        // considers the worker as solved
        // TODO: update GameManager and UI

        GetComponent<InventorySystem>().ReverseEquipment();
        Destroy(GetComponent<Clickable>());
    }

    public void OnQuizEnd(bool answeredCorrectly)
    {
        // callback from the quiz being answered

        if (answeredCorrectly)
        {
            Solve();
        }
        else
        {
            KillWorker();
        }
    }
}
