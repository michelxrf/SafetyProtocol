using UnityEngine;

[RequireComponent(typeof(PatrolPoint))]
public class Workstation : MonoBehaviour
{
    // component attached to some patrol points to expand them to work as workstations
    // TODO: define Accident Type
    // TODO: add station's anim controls

    public GameObject workerLookAt;
    [HideInInspector] public bool isWorkerPresent = false;
    [HideInInspector] public PatrolPoint assossiatedPatrolPoint;
    public string workAnimation = "Work";
    public string assossiatedAccident = "Accident";

    private void Awake()
    {
        assossiatedPatrolPoint = GetComponent<PatrolPoint>();

        if (workerLookAt == null )
        {
            workerLookAt = GetComponentInChildren<MeshRenderer>().gameObject;
        }
    }
}
