using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 20f, -20f);
    public float followLerp = 8f;
    public float rotationLerp = 8f;
    public bool lookAtTarget = false;
    public Vector3 lookAtOffset = Vector3.zero;

    [Tooltip("Top-down orthographic mode: stay fixed at arena center, ignore follow.")]
    public bool fixedOverview = true;
    public Vector3 fixedPosition = new Vector3(0f, 20f, 0f);

    [Header("Zoom")]
    public bool enableZoom = false;
    public float zoomSpeed = 120f;
    public float minOrthographicSize = 60f;
    public float maxOrthographicSize = 500f;
    public float minFieldOfView = 30f;
    public float maxFieldOfView = 75f;
    public KeyCode zoomInKey = KeyCode.Equals;
    public KeyCode zoomOutKey = KeyCode.Minus;

    private Camera cameraComponent;

    void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (fixedOverview)
        {
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            ApplyZoom();
            return;
        }

        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);

        if (lookAtTarget)
        {
            Vector3 lookPoint = target.position + lookAtOffset;
            Vector3 toTarget = lookPoint - transform.position;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationLerp * Time.deltaTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        ApplyZoom();
    }

    private void ApplyZoom()
    {
        if (!enableZoom) return;

        if (cameraComponent == null)
            cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null) return;

        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (Input.GetKey(zoomInKey))
            zoomDelta += zoomSpeed * Time.deltaTime;
        if (Input.GetKey(zoomOutKey))
            zoomDelta -= zoomSpeed * Time.deltaTime;

        if (Mathf.Abs(zoomDelta) <= 0.0001f) return;

        if (cameraComponent.orthographic)
        {
            cameraComponent.orthographicSize = Mathf.Clamp(
                cameraComponent.orthographicSize - zoomDelta,
                minOrthographicSize,
                maxOrthographicSize);
        }
        else
        {
            cameraComponent.fieldOfView = Mathf.Clamp(
                cameraComponent.fieldOfView - zoomDelta,
                minFieldOfView,
                maxFieldOfView);
        }
    }
}
