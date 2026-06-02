using UnityEngine;

[ExecuteInEditMode]
public class OrbitDebugDisplay : MonoBehaviour
{
    public bool showOrbits = true;
    public bool showGravityField = true;
    public int numSteps = 10000;
    public float timeStep = 1f;

    public bool relativeToBody;
    public NBodyCelestialBody centralBody;
    public bool useThickLines;
    public float lineWidth = 0.15f;

    void Start()
    {
        if (Application.isPlaying)
            HideOrbits();
    }

    void Update()
    {
        if (Application.isPlaying) return;

        if (showOrbits)
            DrawOrbits();
        else
            HideOrbits();
    }

    void DrawOrbits()
    {
#if UNITY_6000_0_OR_NEWER
        NBodyCelestialBody[] bodies = FindObjectsByType<NBodyCelestialBody>(FindObjectsSortMode.None);
#else
        NBodyCelestialBody[] bodies = FindObjectsOfType<NBodyCelestialBody>();
#endif
        if (bodies.Length == 0) return;

        var virtualBodies = new VirtualBody[bodies.Length];
        var drawPoints = new Vector3[bodies.Length][];
        int referenceFrameIndex = 0;
        Vector3 referenceBodyInitialPosition = Vector3.zero;

        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[numSteps];

            if (bodies[i] == centralBody && relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].position;
            }
        }

        for (int step = 0; step < numSteps; step++)
        {
            Vector3 referenceBodyPosition = relativeToBody ? virtualBodies[referenceFrameIndex].position : Vector3.zero;

            for (int i = 0; i < virtualBodies.Length; i++)
            {
                if (virtualBodies[i].pinned) continue;
                virtualBodies[i].velocity += CalculateAcceleration(i, virtualBodies) * timeStep;
            }

            for (int i = 0; i < virtualBodies.Length; i++)
            {
                if (virtualBodies[i].pinned)
                {
                    drawPoints[i][step] = virtualBodies[i].position;
                    continue;
                }

                Vector3 newPos = virtualBodies[i].position + virtualBodies[i].velocity * timeStep;
                virtualBodies[i].position = newPos;

                if (relativeToBody)
                {
                    newPos -= referenceBodyPosition - referenceBodyInitialPosition;
                }
                if (relativeToBody && i == referenceFrameIndex)
                {
                    newPos = referenceBodyInitialPosition;
                }

                drawPoints[i][step] = newPos;
            }
        }

        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
        {
            Color pathColour = GetBodyColor(bodies[bodyIndex]);

            if (useThickLines)
            {
                var lr = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                if (lr != null)
                {
                    lr.enabled = true;
                    lr.positionCount = drawPoints[bodyIndex].Length;
                    lr.SetPositions(drawPoints[bodyIndex]);
                    lr.startColor = pathColour;
                    lr.endColor = pathColour;
                    lr.startWidth = lineWidth;
                    lr.endWidth = lineWidth;
                }
            }
            else
            {
                for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++)
                    Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);

                var lr = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                if (lr != null)
                    lr.enabled = false;
            }
        }
    }

    Vector3 CalculateAcceleration(int i, VirtualBody[] virtualBodies)
    {
        Vector3 acceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Length; j++)
        {
            if (i == j) continue;
            acceleration += NBodyGravity.Calculate(
                virtualBodies[i].position, virtualBodies[j].position,
                virtualBodies[j].mass, virtualBodies[j].radius, GravityMode.Center);
        }
        return acceleration;
    }

    void HideOrbits()
    {
#if UNITY_6000_0_OR_NEWER
        NBodyCelestialBody[] bodies = FindObjectsByType<NBodyCelestialBody>(FindObjectsSortMode.None);
#else
        NBodyCelestialBody[] bodies = FindObjectsOfType<NBodyCelestialBody>();
#endif
        for (int i = 0; i < bodies.Length; i++)
        {
            var lr = bodies[i].gameObject.GetComponentInChildren<LineRenderer>();
            if (lr != null)
                lr.positionCount = 0;
        }
    }

    Color GetBodyColor(NBodyCelestialBody body)
    {
        var mr = body.GetComponentInChildren<MeshRenderer>();
        if (mr != null && mr.sharedMaterial != null)
        {
            Color c = mr.sharedMaterial.color;
            if (c.r > 0.4f && c.g > 0.4f && c.b > 0.4f && Mathf.Abs(c.r - c.g) < 0.1f && Mathf.Abs(c.g - c.b) < 0.1f)
                return Color.cyan;
            return c;
        }
        return Color.cyan;
    }

    void OnDrawGizmos()
    {
        if (!showGravityField) return;

#if UNITY_6000_0_OR_NEWER
        var bodies = FindObjectsByType<NBodyCelestialBody>(FindObjectsSortMode.None);
#else
        var bodies = FindObjectsOfType<NBodyCelestialBody>();
#endif
        foreach (var body in bodies)
        {
            float G = NBodyUniverse.gravitationalConstant;
            float m = body.mass;
            float r = body.radius;

            Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
            Gizmos.DrawWireSphere(body.transform.position, r);

            for (int i = 1; i <= 4; i++)
            {
                float dist = r + i * 2f;
                float surfDist = dist - r;
                float accel = G * m / (surfDist * surfDist);
                float alpha = Mathf.Clamp01(accel * 0.5f);
                Gizmos.color = new Color(0f, 1f, 1f, alpha * 0.4f);
                Gizmos.DrawWireSphere(body.transform.position, dist);
            }
        }
    }

    class VirtualBody
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;
        public float radius;
        public bool pinned;

        public VirtualBody(NBodyCelestialBody body)
        {
            position = body.transform.position;
            velocity = body.initialVelocity;
            mass = body.mass;
            radius = body.radius;
            pinned = body.pinned;
        }
    }
}
