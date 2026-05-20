using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float thrustForce = 10f;
    public float turnTorque = 6f;
    public float maxAngularSpeed = 2f;

    [Header("Space Feel")]
    public float linearDamping = 0.05f;
    public float angularDamping = 2f;

    [Header("Visuals")]
    [SerializeField] private TrailRenderer[] exhaustTrails;
    [SerializeField] private float trailOnThreshold = 0.05f;

    private Rigidbody rb;
    private float thrustInput;
    private float turnInput;

    public float ThrustInput => thrustInput;
    public float TurnInput => turnInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping;
    }

    public void MoveRocket(float thrust, float rotation)
    {
        thrustInput = Mathf.Clamp(thrust, -1f, 1f);
        turnInput = Mathf.Clamp(rotation, -1f, 1f);
    }

    void FixedUpdate()
    {
        bool boosting = thrustInput > trailOnThreshold;

        if (exhaustTrails != null)
        {
            for (int i = 0; i < exhaustTrails.Length; i++)
            {
                if (exhaustTrails[i] != null)
                    exhaustTrails[i].emitting = boosting;
            }
        }

        rb.AddForce(transform.forward * thrustInput * thrustForce, ForceMode.Acceleration);
        rb.AddTorque(Vector3.up * turnInput * turnTorque, ForceMode.Acceleration);

        if (rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;
        }
    }

    public void ResetRocket(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
        rb.angularVelocity = Vector3.zero;

        if (exhaustTrails != null)
        {
            for (int i = 0; i < exhaustTrails.Length; i++)
            {
                if (exhaustTrails[i] != null)
                {
                    exhaustTrails[i].Clear();
                    exhaustTrails[i].emitting = false;
                }
            }
        }

        thrustInput = 0f;
        turnInput = 0f;
    }
}
