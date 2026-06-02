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

    [Header("3D Movement")]
    [Tooltip("Acceleration used for optional vertical thrust. Only used by callers that pass a vertical input.")]
    public float verticalThrustForce = 10f;

    [Header("Auto Height")]
    [Tooltip("Automatically applies vertical thrust to match a target's Y height when close enough.")]
    public bool enableAutoHeightAssist = false;
    public Transform autoHeightTarget;
    public float autoHeightRange = 60f;
    public float autoHeightGain = 0.04f;
    public float autoHeightDeadZone = 1f;
    public bool autoHeightUsesPlanarDistance = true;

    [Header("Space Feel")]
    public float linearDamping = 1.0f;   // coast-down braking (no reverse); per-scene value overrides this
    public float angularDamping = 2f;

    [Header("Fuel")]
    [Tooltip("When enabled, forward thrust consumes fuel and stops when empty.")]
    public bool enableFuel = false;
    public float maxFuel = 100f;
    [Tooltip("Fuel units consumed per second at full thrust.")]
    public float fuelBurnPerSecond = 8f;
    [SerializeField] private float currentFuel = 100f;

    public float MaxFuel => maxFuel;
    public float CurrentFuel => currentFuel;
    public float FuelRatio => maxFuel > 0.0001f ? Mathf.Clamp01(currentFuel / maxFuel) : 1f;
    public bool IsOutOfFuel => enableFuel && currentFuel <= 0f;

    [Header("Adaptive Rotation")]
    [Tooltip("When facing away from desired heading, how much thrust is allowed (0-1).")]
    [Range(0f, 1f)] public float reverseThrustFactor = 0.25f;

    [Header("Visuals")]
    [SerializeField] private TrailRenderer[] exhaustTrails;
    [SerializeField] private float trailOnThreshold = 0.05f;

    private Rigidbody rb;
    private float thrustInput;
    private float turnInput;
    private float verticalInput;
    private Vector3 directionInput;
    private bool useDirectionInput;
    private Vector3 externalAcceleration;

    public float ThrustInput => thrustInput;
    public float TurnInput => turnInput;
    public float VerticalInput => verticalInput;
    public Vector3 LinearVelocity =>
#if UNITY_6000_0_OR_NEWER
        rb != null ? rb.linearVelocity : Vector3.zero;
#else
        rb != null ? rb.velocity : Vector3.zero;
#endif
    public Vector3 AngularVelocity => rb != null ? rb.angularVelocity : Vector3.zero;

    public void SetExternalAcceleration(Vector3 accel)
    {
        externalAcceleration = accel;
    }

    public void SetAutoHeightTarget(Transform target)
    {
        autoHeightTarget = target;
    }

    public void Refuel(float amount)
    {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0f, maxFuel);
    }

    public void RefuelFull()
    {
        currentFuel = maxFuel;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping;
        rb.inertiaTensor = Vector3.one;
        currentFuel = maxFuel;

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
        MoveRocket(thrust, rotation, 0f);
    }

    public void MoveRocket(float thrust, float rotation, float vertical)
    {
        thrustInput = Mathf.Clamp(thrust, -1f, 1f);
        turnInput = Mathf.Clamp(rotation, -1f, 1f);
        verticalInput = Mathf.Clamp(vertical, -1f, 1f);
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
        float verticalOut = verticalInput;

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
            verticalOut = 0f;
            verticalInput = 0f;
        }

        if (enableAutoHeightAssist)
        {
            verticalOut = GetAutoHeightInput(verticalOut);
            verticalInput = verticalOut;
        }

        if (enableFuel)
        {
            if (currentFuel <= 0f)
            {
                currentFuel = 0f;
                thrustOut = 0f; // out of fuel: no thrust, rotation (reaction wheels) still allowed
            }
            else
            {
                float burnInput = Mathf.Max(Mathf.Abs(thrustOut), Mathf.Abs(verticalOut));
                float burn = burnInput * fuelBurnPerSecond * Time.fixedDeltaTime;
                currentFuel = Mathf.Max(0f, currentFuel - burn);
            }
            thrustInput = thrustOut;
            verticalInput = verticalOut;
        }

        bool boosting = thrustOut > trailOnThreshold || Mathf.Abs(verticalOut) > trailOnThreshold;
        if (exhaustTrails != null)
        {
            for (int i = 0; i < exhaustTrails.Length; i++)
                if (exhaustTrails[i] != null) exhaustTrails[i].emitting = boosting;
        }

        rb.AddForce(transform.forward * thrustOut * thrustForce, ForceMode.Acceleration);
        rb.AddForce(Vector3.up * verticalOut * verticalThrustForce, ForceMode.Acceleration);
        rb.AddTorque(Vector3.up * torqueOut * turnTorque, ForceMode.Acceleration);

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity += externalAcceleration * Time.fixedDeltaTime;
#else
        rb.velocity += externalAcceleration * Time.fixedDeltaTime;
#endif
        externalAcceleration = Vector3.zero;

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
        verticalInput = 0f;
        directionInput = Vector3.zero;
        useDirectionInput = false;
        externalAcceleration = Vector3.zero;
        autoHeightTarget = null;
        currentFuel = maxFuel;
    }

    private float GetAutoHeightInput(float fallbackInput)
    {
        if (autoHeightTarget == null)
            return fallbackInput;

        Vector3 toTarget = autoHeightTarget.position - transform.position;
        float approachDistance = autoHeightUsesPlanarDistance
            ? new Vector2(toTarget.x, toTarget.z).magnitude
            : toTarget.magnitude;
        if (approachDistance > Mathf.Max(0f, autoHeightRange))
            return fallbackInput;

        if (Mathf.Abs(toTarget.y) <= Mathf.Max(0f, autoHeightDeadZone))
            return 0f;

        return Mathf.Clamp(toTarget.y * autoHeightGain, -1f, 1f);
    }
}
