using UnityEngine;
using System.Collections.Generic;

public class DemoSceneSetup : MonoBehaviour
{
    [Header("Background")]
    public Color backgroundColor = new Color(0.25f, 0.22f, 0.2f, 1f);

    [Header("Arena Boundary")]
    public Color boundaryColor = new Color(0.55f, 0.55f, 0.6f, 0.9f);
    public float boundaryWidth = 0.5f;

    [Header("Player Indicators")]
    public Color ringColor = new Color(0.2f, 0.8f, 1f, 0.6f);
    public Color arrowColor = new Color(0.3f, 1f, 0.5f, 0.8f);
    public float ringRadius = 2.5f;

    [Header("Debris Indicators")]
    public Color debrisMarkerColor = new Color(1f, 0.85f, 0.25f, 0.75f);
    public float debrisMarkerSize = 1.2f;

    [Header("Target Line")]
    public Color targetLineColor = new Color(1f, 0.45f, 0.15f, 0.35f);

    private EnvironmentManager env;
    private Transform player;
    private LineRenderer boundaryLR;
    private LineRenderer ringLR;
    private LineRenderer arrowLR;
    private LineRenderer targetLR;
    private readonly List<LineRenderer> debrisLRs = new List<LineRenderer>();
    private Material lineMat;

    void Awake()
    {
        for (int i = 1; i < Display.displays.Length && i < 3; i++)
            Display.displays[i].Activate();

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = backgroundColor;
        }
    }

    void Start()
    {
        env = FindFirstObjectByType<EnvironmentManager>();
        SpaceCleaningAgent agent = FindFirstObjectByType<SpaceCleaningAgent>();
        if (agent != null) player = agent.transform;

        lineMat = new Material(Shader.Find("Sprites/Default"));

        boundaryLR = MakeLR("Boundary", boundaryColor, boundaryWidth, true, 4);
        ringLR = MakeLR("PlayerRing", ringColor, 0.15f, true, 48);
        arrowLR = MakeLR("HeadingArrow", arrowColor, 0.18f, false, 3);
        targetLR = MakeLR("TargetLine", targetLineColor, 0.12f, false, 2);
    }

    LineRenderer MakeLR(string goName, Color c, float w, bool loop, int pts)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(transform);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = loop;
        lr.positionCount = pts;
        lr.startWidth = w;
        lr.endWidth = w;
        lr.material = lineMat;
        lr.startColor = c;
        lr.endColor = c;
        lr.sortingOrder = 1;
        return lr;
    }

    void LateUpdate()
    {
        float y = 0.1f;

        if (env != null && boundaryLR != null)
        {
            boundaryLR.SetPosition(0, new Vector3(env.ArenaMinX, y, env.ArenaMinZ));
            boundaryLR.SetPosition(1, new Vector3(env.ArenaMaxX, y, env.ArenaMinZ));
            boundaryLR.SetPosition(2, new Vector3(env.ArenaMaxX, y, env.ArenaMaxZ));
            boundaryLR.SetPosition(3, new Vector3(env.ArenaMinX, y, env.ArenaMaxZ));
        }

        if (player != null && ringLR != null)
        {
            Vector3 p = player.position;
            float r = ringRadius * (1f + Mathf.Sin(Time.time * 3f) * 0.06f);
            int n = ringLR.positionCount;
            for (int i = 0; i < n; i++)
            {
                float a = (float)i / n * Mathf.PI * 2f;
                ringLR.SetPosition(i, new Vector3(p.x + Mathf.Cos(a) * r, y, p.z + Mathf.Sin(a) * r));
            }
        }

        if (player != null && arrowLR != null)
        {
            Vector3 p = player.position;
            Vector3 f = player.forward;
            float len = ringRadius + 0.8f;
            Vector3 tip = new Vector3(p.x + f.x * len, y, p.z + f.z * len);
            Vector3 bl = Quaternion.Euler(0, 155, 0) * f * 0.7f;
            Vector3 br = Quaternion.Euler(0, -155, 0) * f * 0.7f;
            arrowLR.SetPosition(0, new Vector3(tip.x + bl.x, y, tip.z + bl.z));
            arrowLR.SetPosition(1, tip);
            arrowLR.SetPosition(2, new Vector3(tip.x + br.x, y, tip.z + br.z));
        }

        if (player != null && env != null && targetLR != null)
        {
            Transform near = env.GetNearestDebris(player.position);
            targetLR.enabled = near != null;
            if (near != null)
            {
                targetLR.SetPosition(0, new Vector3(player.position.x, y, player.position.z));
                targetLR.SetPosition(1, new Vector3(near.position.x, y, near.position.z));
            }
        }

        UpdateDebrisMarkers(y);
    }

    void UpdateDebrisMarkers(float y)
    {
        if (env == null) return;

        GameObject[] debris = GameObject.FindGameObjectsWithTag("Debris");

        while (debrisLRs.Count < debris.Length)
            debrisLRs.Add(MakeLR("DM" + debrisLRs.Count, debrisMarkerColor, 0.12f, true, 4));

        for (int i = 0; i < debrisLRs.Count; i++)
        {
            if (i < debris.Length && debris[i] != null)
            {
                debrisLRs[i].enabled = true;
                Vector3 dp = debris[i].transform.position;
                float s = debrisMarkerSize * (1f + Mathf.Sin(Time.time * 2f + i * 1.3f) * 0.12f);
                debrisLRs[i].SetPosition(0, new Vector3(dp.x, y, dp.z + s));
                debrisLRs[i].SetPosition(1, new Vector3(dp.x + s, y, dp.z));
                debrisLRs[i].SetPosition(2, new Vector3(dp.x, y, dp.z - s));
                debrisLRs[i].SetPosition(3, new Vector3(dp.x - s, y, dp.z));
            }
            else
            {
                debrisLRs[i].enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        if (lineMat != null) Destroy(lineMat);
    }
}