using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ThirdPersonCameraControllScript : MonoBehaviour
{

    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;

    public InputActionReference zoom;

    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        orbital = GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    private void Update()
    {
        if (orbital == null)
        {
            return;
        }

        scrollDelta = zoom.action.ReadValue<Vector2>();

        if (Mathf.Abs(scrollDelta.y) > 0.01f)
        {
            targetZoom -= Mathf.Sign(scrollDelta.y) * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minDistance, maxDistance);
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomLerpSpeed * Time.deltaTime);
        orbital.Radius = currentZoom;
    }
}