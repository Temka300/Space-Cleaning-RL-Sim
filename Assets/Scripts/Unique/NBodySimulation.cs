using UnityEngine;

public class NBodySimulation : MonoBehaviour
{
    [Range(0.1f, 20f)]
    public float simulationSpeed = 1f;

    NBodyCelestialBody[] bodies;
    static NBodySimulation instance;

    void Awake()
    {
#if UNITY_6000_0_OR_NEWER
        bodies = FindObjectsByType<NBodyCelestialBody>(FindObjectsSortMode.None);
#else
        bodies = FindObjectsOfType<NBodyCelestialBody>();
#endif
        Time.fixedDeltaTime = NBodyUniverse.physicsTimeStep;
    }

    void FixedUpdate()
    {
        float dt = NBodyUniverse.physicsTimeStep * simulationSpeed;

        for (int i = 0; i < bodies.Length; i++)
        {
            Vector3 acceleration = CalculateAcceleration(bodies[i].Position, bodies[i]);
            bodies[i].UpdateVelocity(acceleration, dt);
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePosition(dt);
        }
    }

    public static Vector3 CalculateAcceleration(Vector3 point, NBodyCelestialBody ignoreBody = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in Instance.bodies)
        {
            if (body != ignoreBody)
                acceleration += NBodyGravity.Calculate(point, body.Position, body.mass, body.radius, GravityMode.Center);
        }
        return acceleration;
    }

    public static NBodyCelestialBody[] Bodies
    {
        get { return Instance.bodies; }
    }

    static NBodySimulation Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_6000_0_OR_NEWER
                instance = FindAnyObjectByType<NBodySimulation>();
#else
                instance = FindObjectOfType<NBodySimulation>();
#endif
            }
            return instance;
        }
    }
}
