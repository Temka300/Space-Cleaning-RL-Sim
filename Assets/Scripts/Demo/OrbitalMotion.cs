using UnityEngine;

public class OrbitalMotion : MonoBehaviour
{
    [Header("Orbit")]
    public Transform orbitCenter;
    public float orbitRadiusX = 20f;
    public float orbitRadiusZ = 15f;
    public float orbitalSpeedDegPerSec = 10f;
    public bool clockwise = false;

    [Header("Start")]
    [Tooltip("If true, computes starting angle from current scene position.")]
    public bool useCurrentPositionAsStart = true;
    public float startAngleDegrees = 0f;

    [Header("Orbit Path Visual")]
    public bool drawOrbitPath = true;
    public Color orbitPathColor = new Color(0.3f, 0.3f, 0.4f, 0.25f);
    public float orbitPathWidth = 0.1f;

    private float currentAngle;
    private float orbitY;

    void Start()
    {
        if (orbitCenter == null)
        {
            GameObject sun = GameObject.Find("Sun");
            if (sun != null) orbitCenter = sun.transform;
        }

        if (orbitCenter == null) return;

        orbitY = transform.position.y;

        if (useCurrentPositionAsStart)
        {
            Vector3 offset = transform.position - orbitCenter.position;
            offset.y = 0f;
            currentAngle = Mathf.Atan2(
                offset.z / Mathf.Max(0.01f, orbitRadiusZ),
                offset.x / Mathf.Max(0.01f, orbitRadiusX));
        }
        else
        {
            currentAngle = startAngleDegrees * Mathf.Deg2Rad;
        }

        SnapToOrbit();

        if (drawOrbitPath)
            CreateOrbitPath();
    }

    void Update()
    {
        if (orbitCenter == null) return;
        float dir = clockwise ? -1f : 1f;
        currentAngle += dir * orbitalSpeedDegPerSec * Mathf.Deg2Rad * Time.deltaTime;
        SnapToOrbit();
    }

    void SnapToOrbit()
    {
        transform.position = new Vector3(
            orbitCenter.position.x + Mathf.Cos(currentAngle) * orbitRadiusX,
            orbitY,
            orbitCenter.position.z + Mathf.Sin(currentAngle) * orbitRadiusZ);
    }

    void CreateOrbitPath()
    {
        GameObject go = new GameObject("OrbitPath_" + gameObject.name);
        go.transform.SetParent(orbitCenter != null ? orbitCenter : transform);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        int segs = 64;
        lr.positionCount = segs;
        lr.startWidth = orbitPathWidth;
        lr.endWidth = orbitPathWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = orbitPathColor;
        lr.endColor = orbitPathColor;
        lr.sortingOrder = 0;

        Vector3 center = orbitCenter != null ? orbitCenter.position : Vector3.zero;
        for (int i = 0; i < segs; i++)
        {
            float a = (float)i / segs * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(
                center.x + Mathf.Cos(a) * orbitRadiusX,
                orbitY,
                center.z + Mathf.Sin(a) * orbitRadiusZ));
        }
    }
}