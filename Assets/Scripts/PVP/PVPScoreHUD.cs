using UnityEngine;

public enum PVPFuelGaugeMode
{
    CanisterPickups,
    FuelTank
}

public class PVPScoreHUD : MonoBehaviour
{
    public PVPEnvironmentManager pvpEnvironment;

    [Header("Style")]
    public Color teamAColor = new Color(0.2f, 0.6f, 1f);
    public Color teamBColor = new Color(1f, 0.3f, 0.3f);
    public int fontSize = 28;

    [Header("Fuel Gauges")]
    [SerializeField] private PVPAgent teamAAgent;
    [SerializeField] private PVPAgent teamBAgent;
    [SerializeField] private bool showFuelGauges = true;
    [SerializeField] private PVPFuelGaugeMode fuelGaugeMode = PVPFuelGaugeMode.CanisterPickups;
    [SerializeField] private Vector2 fuelGaugeSize = new Vector2(48f, 78f);
    [SerializeField] private float fuelGaugeThickness = 14f;
    [SerializeField] private int fuelGaugeSegments = 24;
    [SerializeField] private float fuelGaugeTextGap = 20f;
    [SerializeField] private float fuelGaugeYOffset = -2f;
    [SerializeField] private Color emptyFuelColor = new Color(1f, 1f, 1f, 0.16f);

    private GUIStyle styleA;
    private GUIStyle styleB;
    private GUIStyle styleCenter;
    private Texture2D circleTexture;

    void OnGUI()
    {
        if (pvpEnvironment == null) return;
        ResolveAgents();

        if (styleA == null)
        {
            styleA = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            styleA.normal.textColor = teamAColor;

            styleB = new GUIStyle(styleA)
            {
                alignment = TextAnchor.UpperRight
            };
            styleB.normal.textColor = teamBColor;

            styleCenter = new GUIStyle(styleA)
            {
                alignment = TextAnchor.UpperCenter
            };
            styleCenter.normal.textColor = Color.white;
        }

        float w = Screen.width;
        float pad = 20f;
        float h = 50f;
        float leftLabelWidth = w * 0.3f;
        float rightLabelX = w * 0.7f - pad;
        float rightLabelWidth = w * 0.3f;

        GUI.Label(new Rect(pad, pad, leftLabelWidth, h),
            $"BLUE: {pvpEnvironment.ScoreA}", styleA);

        GUI.Label(new Rect(w * 0.35f, pad, w * 0.3f, h),
            $"Debris: {pvpEnvironment.RemainingDebris}", styleCenter);

        GUI.Label(new Rect(rightLabelX, pad, rightLabelWidth, h),
            $"RED: {pvpEnvironment.ScoreB}", styleB);

        if (!showFuelGauges) return;

        float scale = Mathf.Clamp(Screen.height / 1080f, 0.7f, 1.25f);
        Vector2 gaugeSize = fuelGaugeSize * scale;
        float gaugeY = pad + fuelGaugeYOffset * scale;
        float leftGaugeX = pad + styleA.CalcSize(new GUIContent($"BLUE: {pvpEnvironment.ScoreA}")).x
            + fuelGaugeTextGap * scale;
        float rightGaugeX = rightLabelX + rightLabelWidth
            - styleB.CalcSize(new GUIContent($"RED: {pvpEnvironment.ScoreB}")).x
            - fuelGaugeTextGap * scale
            - gaugeSize.x;

        DrawFuelGauge(new Rect(leftGaugeX, gaugeY, gaugeSize.x, gaugeSize.y),
            GetGaugeRatio(teamAAgent, 0), teamAColor, true);
        DrawFuelGauge(new Rect(rightGaugeX, gaugeY, gaugeSize.x, gaugeSize.y),
            GetGaugeRatio(teamBAgent, 1), teamBColor, false);
    }

    private void ResolveAgents()
    {
        if (teamAAgent != null && teamBAgent != null) return;

#if UNITY_6000_0_OR_NEWER
        PVPAgent[] agents = FindObjectsByType<PVPAgent>(FindObjectsSortMode.None);
#else
        PVPAgent[] agents = FindObjectsOfType<PVPAgent>();
#endif
        foreach (PVPAgent agent in agents)
        {
            if (agent == null) continue;
            if (agent.team == 0 && teamAAgent == null)
                teamAAgent = agent;
            else if (agent.team == 1 && teamBAgent == null)
                teamBAgent = agent;
        }
    }

    private float GetGaugeRatio(PVPAgent agent, int team)
    {
        if (fuelGaugeMode == PVPFuelGaugeMode.CanisterPickups
            && pvpEnvironment != null
            && pvpEnvironment.fuelCanisterCount > 0)
        {
            return pvpEnvironment.GetFuelPickupRatio(team);
        }

        return GetFuelRatio(agent);
    }

    private static float GetFuelRatio(PVPAgent agent)
    {
        if (agent == null || agent.rocketMovement == null)
            return 1f;
        return agent.rocketMovement.FuelRatio;
    }

    private void DrawFuelGauge(Rect rect, float fuelRatio, Color fuelColor, bool rightFacing)
    {
        EnsureCircleTexture();

        int segmentCount = Mathf.Max(8, fuelGaugeSegments);
        float dotSize = Mathf.Max(4f, fuelGaugeThickness * Mathf.Clamp(Screen.height / 1080f, 0.7f, 1.25f));
        float rx = Mathf.Max(1f, (rect.width - dotSize) * 0.42f);
        float ry = Mathf.Max(1f, (rect.height - dotSize) * 0.42f);
        float centerX = rect.x + rect.width * 0.5f;
        float centerY = rect.y + rect.height * 0.5f;
        float side = rightFacing ? 1f : -1f;
        float clampedFuel = Mathf.Clamp01(fuelRatio);

        Color previousColor = GUI.color;
        for (int i = 0; i < segmentCount; i++)
        {
            float positionT = segmentCount == 1 ? 0.5f : (float)i / (segmentCount - 1);
            float fillT = (i + 1f) / segmentCount;
            float angle = Mathf.Lerp(70f, -70f, positionT) * Mathf.Deg2Rad;
            float x = centerX + side * Mathf.Cos(angle) * rx - dotSize * 0.5f;
            float y = centerY + Mathf.Sin(angle) * ry - dotSize * 0.5f;
            Rect dotRect = new Rect(x, y, dotSize, dotSize);

            GUI.color = emptyFuelColor;
            GUI.DrawTexture(dotRect, circleTexture);

            if (fillT <= clampedFuel)
            {
                GUI.color = fuelColor;
                GUI.DrawTexture(dotRect, circleTexture);
            }
        }
        GUI.color = previousColor;
    }

    private void EnsureCircleTexture()
    {
        if (circleTexture != null) return;

        const int size = 32;
        circleTexture = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        float radius = (size - 1) * 0.5f;
        Vector2 center = new Vector2(radius, radius);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(radius + 0.5f - distance);
                circleTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        circleTexture.Apply(false, true);
    }
}
