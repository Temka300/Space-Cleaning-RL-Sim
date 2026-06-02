using UnityEngine;

// Collectible propellant canister. When a rocket touches it, it refills fuel.
// Mirrors SpaceDebris drifting/bounce behaviour so canisters can be static or drifting.
public class PropellantCanister : MonoBehaviour
{
    [Tooltip("Fuel units restored on pickup. <= 0 means refill to full.")]
    public float refillAmount = -1f;

    private Rigidbody rb;
    public Rigidbody Rb => rb;

    private float minX, maxX, minY, maxY, minZ, maxZ;
    private bool hasBounds;
    private bool hasVerticalBounds;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = false;
    }

    // Apply this canister's refill to a rocket. Returns true if it actually refilled.
    public bool ApplyTo(RocketMovement rocket)
    {
        if (rocket == null) return false;
        if (refillAmount <= 0f) rocket.RefuelFull();
        else rocket.Refuel(refillAmount);
        return true;
    }

    public void SetDriftVelocity(Vector3 velocity)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = velocity;
#else
            rb.velocity = velocity;
#endif
        }
    }

    public void MakeStatic()
    {
        if (rb != null) rb.isKinematic = true;
    }

    public void SetBounds(float minXWorld, float maxXWorld, float minZWorld, float maxZWorld)
    {
        minX = minXWorld; maxX = maxXWorld; minZ = minZWorld; maxZ = maxZWorld;
        hasVerticalBounds = false;
        hasBounds = true;
    }

    public void SetBounds(float minXWorld, float maxXWorld, float minYWorld, float maxYWorld, float minZWorld, float maxZWorld)
    {
        minX = minXWorld; maxX = maxXWorld; minY = minYWorld; maxY = maxYWorld; minZ = minZWorld; maxZ = maxZWorld;
        hasVerticalBounds = true;
        hasBounds = true;
    }

    void FixedUpdate()
    {
        if (!hasBounds || rb == null || rb.isKinematic) return;

#if UNITY_6000_0_OR_NEWER
        Vector3 v = rb.linearVelocity;
#else
        Vector3 v = rb.velocity;
#endif
        bool changed = false;
        Vector3 pos = transform.position;
        if (pos.x > maxX && v.x > 0f) { v.x = -v.x; changed = true; }
        else if (pos.x < minX && v.x < 0f) { v.x = -v.x; changed = true; }
        if (hasVerticalBounds)
        {
            if (pos.y > maxY && v.y > 0f) { v.y = -v.y; changed = true; }
            else if (pos.y < minY && v.y < 0f) { v.y = -v.y; changed = true; }
        }
        if (pos.z > maxZ && v.z > 0f) { v.z = -v.z; changed = true; }
        else if (pos.z < minZ && v.z < 0f) { v.z = -v.z; changed = true; }

        if (changed)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = v;
#else
            rb.velocity = v;
#endif
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            if (hasVerticalBounds)
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            transform.position = pos;
        }
    }
}
