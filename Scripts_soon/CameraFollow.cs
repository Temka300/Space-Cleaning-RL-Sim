using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 20f, 0f);
    public float followLerp = 8f;

    [Tooltip("Top-down orthographic mode: stay fixed at arena center, ignore follow.")]
    public bool fixedOverview = true;
    public Vector3 fixedPosition = new Vector3(0f, 20f, 0f);

    void LateUpdate()
    {
        if (fixedOverview)
        {
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            return;
        }

        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
