using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class NBodyCelestialBody : MonoBehaviour
{
    public float radius;
    public float surfaceGravity;
    public Vector3 initialVelocity;
    public string bodyName = "Unnamed";
    public bool pinned;

    public Vector3 velocity { get; private set; }
    public Vector3 Velocity => velocity;
    public float mass { get; private set; }

    Rigidbody rb;
    Transform meshHolder;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mass = ComputeMass();
        rb.mass = mass;
        rb.useGravity = false;
        rb.isKinematic = true;
        velocity = initialVelocity;
        SyncVisualAndSurfaceRadius();
    }

    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        if (pinned) return;
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        if (pinned) return;
        rb.MovePosition(rb.position + velocity * timeStep);
    }

    void OnValidate()
    {
        mass = ComputeMass();
        SyncVisualAndSurfaceRadius();
    }

    private float ComputeMass()
    {
        return surfaceGravity * radius * radius / NBodyUniverse.gravitationalConstant;
    }

    private void SyncVisualAndSurfaceRadius()
    {
        if (transform.childCount > 0)
        {
            meshHolder = transform.GetChild(0);
            meshHolder.localScale = Vector3.one * radius;
        }

        float surfaceRadius = GetVisibleSurfaceRadius();

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            float localScale = Mathf.Max(
                Mathf.Abs(transform.lossyScale.x),
                Mathf.Abs(transform.lossyScale.z),
                0.0001f);
            sphereCollider.radius = surfaceRadius / localScale;
        }

        GravitySource gravitySource = GetComponent<GravitySource>();
        if (gravitySource != null)
        {
            float previousSurfaceRadius = Mathf.Max(0.0001f, gravitySource.minimumDistance);
            float clearance = Mathf.Max(0f, gravitySource.spawnExclusionRadius - previousSurfaceRadius);
            gravitySource.minimumDistance = surfaceRadius;
            gravitySource.spawnExclusionRadius = surfaceRadius + clearance;
        }

        gameObject.name = bodyName;
    }

    private float GetVisibleSurfaceRadius()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || renderer is LineRenderer)
                continue;

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!hasBounds)
            return Mathf.Max(0.0001f, radius * 0.5f);

        return Mathf.Max(0.0001f, Mathf.Max(bounds.extents.x, bounds.extents.z));
    }

    public Vector3 Position
    {
        get
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            return rb.position;
        }
    }

    public void ResetState(Vector3 position, Quaternion rotation, Vector3 newVelocity)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        transform.SetPositionAndRotation(position, rotation);
        rb.position = position;
        rb.rotation = rotation;
        velocity = newVelocity;

        if (!rb.isKinematic)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }
    }

}
