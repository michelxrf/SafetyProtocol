using UnityEngine;

[RequireComponent(typeof(PatrolPoint))]
public class Workstation : MonoBehaviour
{
    // component attached to some patrol points to expand them to work as workstations

    public GameObject workerLookAt;
    [HideInInspector] public PatrolPoint assossiatedPatrolPoint;
    public Worker.JOB_TYPE workerType;
    public string workAnimation = "Work";
    [HideInInspector] public Camera viewPortCamera;

    private void Awake()
    {
        assossiatedPatrolPoint = GetComponent<PatrolPoint>();
        viewPortCamera = GetComponentInChildren<Camera>();
        viewPortCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (workerLookAt == null)
        {
            workerLookAt = GetComponentInChildren<MeshRenderer>().gameObject;
        }
    }
}
