using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -3f);
    public float followLerp = 8f;
    public float rotationLerp = 6f;
    public Vector3 lookOffset = new Vector3(0f, 0f, 0f);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);

        Vector3 lookAt = target.position + lookOffset;
        Quaternion desiredRot = Quaternion.LookRotation(lookAt - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationLerp * Time.deltaTime);
    }
}
