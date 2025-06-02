using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentDestinationReachedNotifier : MonoBehaviour
{
    // Notifies listeners when the agent reaches it's destination
    // because Unity still doesn't have such a basic function built in


    private NavMeshAgent navMeshAgent;
    private bool hasArrived = false;
    public event Action OnDestinationReached;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    if (!hasArrived)
                    {
                        hasArrived = true;
                        OnDestinationReached?.Invoke();
                        Debug.Log("Destination reached!");
                    }
                }
            }
            else
            {
                hasArrived = false;
            }
        }
    }
}
