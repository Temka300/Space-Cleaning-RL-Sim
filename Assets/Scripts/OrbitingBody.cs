using UnityEngine;

/// <summary>
/// Deterministic kinematic orbit. The body travels a fixed circle around
/// <see cref="orbitCenter"/> at a tunable angular speed, driven by a kinematic
/// Rigidbody (MovePosition) so it still collides with the rocket. Unlike the
/// NBody solver, orbits never decay and bodies never perturb each other — assign
/// a center in the inspector and set the speed; that is the whole setup. A moon
/// points <see cref="orbitCenter"/> at its planet, a planet at the sun, the sun
/// at nothing (stays put).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OrbitingBody : MonoBehaviour
{
    [Header("Orbit")]
    [Tooltip("Body to orbit around (e.g. the Sun). Leave empty to stay put.")]
    public Transform orbitCenter;

    [Tooltip("Orbit angular speed in degrees per second. Negative orbits the other way. This is the 'how fast it goes round' knob.")]
    public float orbitSpeedDegPerSec = 20f;

    [Tooltip("Lock the orbit radius to the distance from the center at scene start. Untick to use Orbit Radius directly.")]
    public bool useStartDistanceAsRadius = true;

    [Tooltip("Orbit radius used when 'Use Start Distance As Radius' is off.")]
    public float orbitRadius = 50f;

    [Header("Self Spin (visual only)")]
    [Tooltip("Degrees per second the body spins on its own Y axis. 0 = no spin.")]
    public float spinSpeedDegPerSec = 0f;

    [Header("Editor Preview")]
    [Tooltip("Draw the orbit path in the Scene view (always, not just when selected). Editor only — never shows in the Game view or in builds.")]
    public bool showOrbitPath = true;
    public Color orbitPathColor = new Color(0.3f, 0.8f, 1f, 0.7f);

    private Rigidbody rb;
    private float angleRad;
    private float radius;

    public float Radius => radius;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Start()
    {
        radius = ResolveRadius();
        if (orbitCenter != null)
        {
            Vector3 d = transform.position - orbitCenter.position;
            angleRad = Mathf.Atan2(d.z, d.x);
        }
    }

    void FixedUpdate()
    {
        if (orbitCenter != null && radius > 0.0001f)
        {
            angleRad += orbitSpeedDegPerSec * Mathf.Deg2Rad * Time.fixedDeltaTime;
            Vector3 c = orbitCenter.position;
            Vector3 next = new Vector3(
                c.x + Mathf.Cos(angleRad) * radius,
                transform.position.y,
                c.z + Mathf.Sin(angleRad) * radius);
            rb.MovePosition(next);
        }

        if (spinSpeedDegPerSec != 0f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, spinSpeedDegPerSec * Time.fixedDeltaTime, 0f));
    }

    private float ResolveRadius()
    {
        if (orbitCenter == null) return 0f;
        if (!useStartDistanceAsRadius) return orbitRadius;
        Vector3 d = transform.position - orbitCenter.position;
        d.y = 0f;
        return d.magnitude;
    }

    // Draw the fixed orbit circle in the Scene view so layout is predictable.
    // OnDrawGizmos renders in the Scene view always (off in the Game view by
    // default, never in builds). For a circular kinematic orbit this circle is
    // the exact path the body will travel.
    void OnDrawGizmos()
    {
        if (!showOrbitPath || orbitCenter == null) return;
        float r = ResolveRadius();
        if (r <= 0.0001f) return;

        Gizmos.color = orbitPathColor;
        Vector3 c = orbitCenter.position;
        const int seg = 96;
        Vector3 prev = c + new Vector3(r, 0f, 0f);
        for (int i = 1; i <= seg; i++)
        {
            float a = (i / (float)seg) * Mathf.PI * 2f;
            Vector3 p = c + new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r);
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
        // Radius line from center + a marker at the body's current spot.
        Gizmos.DrawLine(c, transform.position);
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.5f, r * 0.03f));
    }
}
