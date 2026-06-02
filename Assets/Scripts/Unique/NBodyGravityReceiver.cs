using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NBodyGravityReceiver : MonoBehaviour
{
    public GravityMode gravityMode = GravityMode.Surface;
    public float gravityStrength = 1f;
    public float maxAcceleration = 50f;

    Rigidbody rb;
    NBodyCelestialBody[] bodies;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (bodies == null || bodies.Length == 0)
        {
#if UNITY_6000_0_OR_NEWER
            bodies = FindObjectsByType<NBodyCelestialBody>(FindObjectsSortMode.None);
#else
            bodies = FindObjectsOfType<NBodyCelestialBody>();
#endif
        }

        Vector3 acceleration = Vector3.zero;
        foreach (var body in bodies)
        {
            if (body == null) continue;
            acceleration += NBodyGravity.Calculate(
                rb.position, body.transform.position,
                body.mass, body.radius,
                gravityMode, gravityStrength);
        }

        if (acceleration.magnitude > maxAcceleration)
            acceleration = acceleration.normalized * maxAcceleration;

        rb.AddForce(acceleration, ForceMode.Acceleration);
    }
}
