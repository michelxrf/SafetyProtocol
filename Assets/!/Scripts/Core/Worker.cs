using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    // controls the worker

    [Header("References")]
    [SerializeField] private WorkerManager workerManager;
    private AgentDestinationReachedNotifier destinationReachedNotifier;

    [Header("Navigation")]
    private NavMeshAgent navMeshAgent;

    [Header("Work")]
    [SerializeField] public JOB_TYPE workerType;
    [HideInInspector] public STATE currentState;
    public enum STATE { IDLING, MOVING, WORKING };
    public enum JOB_TYPE { CONSTRUCTION, ELECTRICAL, HEIGHT };
    private PatrolPoint assignedPoint;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        destinationReachedNotifier = GetComponent<AgentDestinationReachedNotifier>();
    }

    private void Start()
    {
        MoveToRandomPoint();
    }

    private void OnEnable()
    {
        destinationReachedNotifier.OnDestinationReached += MoveToRandomPoint;
    }

    private void OnDisable()
    {
        destinationReachedNotifier.OnDestinationReached -= MoveToRandomPoint;
    }

    public void MoveToPoint(PatrolPoint patrolPoint)
    {
        // sends the worker to a patrol point on the level

        FreeAssignedPoint();
        assignedPoint = patrolPoint;
        assignedPoint.AssignWorker(this);

        currentState = STATE.MOVING;

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

        Debug.Log($"{gameObject.name} is moving");
        MoveToPoint(workerManager.GetRandomPatrolPoint());
    }
}
