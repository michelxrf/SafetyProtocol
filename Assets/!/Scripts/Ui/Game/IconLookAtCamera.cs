using UnityEngine;


/// <summary>
/// Rotates the icons on top of objectives so they always face the camera.
/// </summary>
public class IconLookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 dir = transform.position - mainCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
