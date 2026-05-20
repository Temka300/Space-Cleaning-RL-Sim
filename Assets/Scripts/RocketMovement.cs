using UnityEngine;

public enum RocketMovementType
{
    ManualRotation,
    AdaptiveRotation
}

public class RocketMovement : MonoBehaviour
{
    [Header("Movement")]
    public RocketMovementType movementType = RocketMovementType.ManualRotation;
    public float thrustForce = 10f;
    public float turnTorque = 6f;
    public float maxAngularSpeed = 2f;
    public float maxLinearSpeed = 8f;

    [Header("Space Feel")]
    public float linearDamping = 0.05f;
    public float angularDamping = 2f;

    [Header("Adaptive Rotation")]
    [Tooltip("When facing away from desired heading, how much thrust is allowed (0-1).")]
    [Range(0f, 1f)] public float reverseThrustFactor = 0.25f;

    [Header("Visuals")]
    [SerializeField] private TrailRenderer[] exhaustTrails;
    [SerializeField] private float trailOnThreshold = 0.05f;

    private Rigidbody rb;
    private float thrustInput;
    private float turnInput;
    private Vector3 directionInput;
    private bool useDirectionInput;

    public float ThrustInput => thrustInput;
    public float TurnInput => turnInput;
    public Vector3 LinearVelocity =>
#if UNITY_6000_0_OR_NEWER
        rb != null ? rb.linearVelocity : Vector3.zero;
#else
        rb != null ? rb.velocity : Vector3.zero;
#endif
    public Vector3 AngularVelocity => rb != null ? rb.angularVelocity : Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping;

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
    }

    public void MoveRocket(float thrust, float rotation)
    {
        thrustInput = Mathf.Clamp(thrust, -1f, 1f);
        turnInput = Mathf.Clamp(rotation, -1f, 1f);
        useDirectionInput = false;
    }

    public void MoveRocketDirection(Vector2 worldXZ)
    {
        directionInput = new Vector3(
            Mathf.Clamp(worldXZ.x, -1f, 1f),
            0f,
            Mathf.Clamp(worldXZ.y, -1f, 1f));
        useDirectionInput = true;
    }

    void FixedUpdate()
    {
        float thrustOut = thrustInput;
        float torqueOut = turnInput;

        if (movementType == RocketMovementType.AdaptiveRotation && useDirectionInput)
        {
            float mag = Mathf.Min(1f, directionInput.magnitude);
            if (mag > 0.001f)
            {
                Vector3 dirN = directionInput.normalized;
                float dot = Vector3.Dot(dirN, transform.forward);
                float side = Vector3.Dot(dirN, transform.right);
                thrustOut = dot >= 0f ? dot * mag : dot * mag * reverseThrustFactor;
                torqueOut = Mathf.Clamp(side, -1f, 1f);
            }
            else
            {
                thrustOut = 0f;
                torqueOut = 0f;
            }
            thrustInput = thrustOut;
            turnInput = torqueOut;
        }

        bool boosting = thrustOut > trailOnThreshold;
        if (exhaustTrails != null)
        {
            for (int i = 0; i < exhaustTrails.Length; i++)
                if (exhaustTrails[i] != null) exhaustTrails[i].emitting = boosting;
        }

        rb.AddForce(transform.forward * thrustOut * thrustForce, ForceMode.Acceleration);
        rb.AddTorque(Vector3.up * torqueOut * turnTorque, ForceMode.Acceleration);

        if (rb.angularVelocity.magnitude > maxAngularSpeed)
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;

#if UNITY_6000_0_OR_NEWER
        Vector3 v = rb.linearVelocity;
#else
        Vector3 v = rb.velocity;
#endif
        if (v.magnitude > maxLinearSpeed)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = v.normalized * maxLinearSpeed;
#else
            rb.velocity = v.normalized * maxLinearSpeed;
#endif
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
        directionInput = Vector3.zero;
        useDirectionInput = false;
    }
}
