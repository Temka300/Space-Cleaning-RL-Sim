using UnityEngine;
using UnityEngine.UI;

public class CleaningFeedbackUI : MonoBehaviour
{
    [Header("Display")]
    public int targetDisplay = 2;

    private EnvironmentManager env;
    private Transform player;
    private RocketMovement rocketMovement;
    private CapsuleCollider playerCollider;

    private Image progressFill;
    private Text progressText;
    private Text directionArrow;
    private RectTransform arrowRT;
    private Text distanceText;
    private Text notifyText;
    private Text rangeText;
    private Text speedText;

    private int lastRemaining = -1;
    private int totalDebris;
    private float notifyTimer;

    void Start()
    {
        env = FindFirstObjectByType<EnvironmentManager>();
        SpaceCleaningAgent agent = FindFirstObjectByType<SpaceCleaningAgent>();
        if (agent != null)
        {
            player = agent.transform;
            rocketMovement = agent.rocketMovement;
            playerCollider = agent.GetComponent<CapsuleCollider>();
        }
        BuildCanvas();
    }

    void BuildCanvas()
    {
        GameObject canvasGO = new GameObject("Feedback_Canvas");
        canvasGO.transform.SetParent(transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.targetDisplay = targetDisplay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        Image bg = AddImage(canvasGO.transform, "BG", new Color(0.08f, 0.08f, 0.1f, 0.9f));
        Stretch(bg.rectTransform);

        AddLabel(canvasGO.transform, "CLEANING FEEDBACK", 36, Color.white,
            TextAnchor.UpperCenter, new Vector2(0f, 0.93f), new Vector2(1f, 1f));

        Image progBG = AddImage(canvasGO.transform, "ProgBG",
            new Color(0.2f, 0.2f, 0.25f, 1f));
        SetAnchors(progBG.rectTransform, new Vector2(0.1f, 0.84f), new Vector2(0.9f, 0.88f));

        progressFill = AddImage(progBG.transform, "ProgFill",
            new Color(0.2f, 0.9f, 0.4f, 1f));
        RectTransform fillRT = progressFill.rectTransform;
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(0f, 1f);
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        progressText = AddLabel(canvasGO.transform, "0%", 24, Color.white,
            TextAnchor.MiddleCenter, new Vector2(0.1f, 0.84f), new Vector2(0.9f, 0.88f));

        directionArrow = AddLabel(canvasGO.transform, "▲", 120,
            new Color(1f, 0.5f, 0.2f),
            TextAnchor.MiddleCenter, new Vector2(0.3f, 0.42f), new Vector2(0.7f, 0.75f));
        arrowRT = directionArrow.rectTransform;

        distanceText = AddLabel(canvasGO.transform, "DISTANCE: --", 28,
            new Color(1f, 0.8f, 0.3f),
            TextAnchor.MiddleCenter, new Vector2(0.15f, 0.34f), new Vector2(0.85f, 0.42f));

        notifyText = AddLabel(canvasGO.transform, "", 48,
            new Color(0.3f, 1f, 0.5f),
            TextAnchor.MiddleCenter, new Vector2(0.1f, 0.18f), new Vector2(0.9f, 0.32f));

        rangeText = AddLabel(canvasGO.transform, "PICKUP RANGE: --", 22,
            new Color(0.7f, 0.7f, 0.7f),
            TextAnchor.LowerLeft, new Vector2(0.04f, 0.02f), new Vector2(0.5f, 0.08f));

        speedText = AddLabel(canvasGO.transform, "SPEED: 0.0", 22,
            new Color(0.7f, 0.7f, 0.7f),
            TextAnchor.LowerRight, new Vector2(0.5f, 0.02f), new Vector2(0.96f, 0.08f));
    }

    void Update()
    {
        if (env == null) return;

        int remaining = env.RemainingDebris;
        int total = env.debrisCount;

        if (remaining > lastRemaining && lastRemaining >= 0)
            totalDebris = total;

        if (lastRemaining > remaining && remaining >= 0 && lastRemaining > 0)
        {
            int just = lastRemaining - remaining;
            notifyTimer = 1.5f;
            notifyText.text = "COLLECTED! (+" + just + ")";
            notifyText.color = new Color(0.3f, 1f, 0.5f, 1f);
        }

        if (totalDebris == 0) totalDebris = total;
        lastRemaining = remaining;

        int collected = Mathf.Max(0, totalDebris - remaining);
        float progress = totalDebris > 0 ? (float)collected / totalDebris : 0f;
        progressFill.rectTransform.anchorMax = new Vector2(progress, 1f);
        progressText.text = (int)(progress * 100f) + "%  (" + collected + "/" + totalDebris + ")";

        if (player != null)
        {
            Transform nearest = env.GetNearestDebris(player.position);
            if (nearest != null)
            {
                Vector3 dir = nearest.position - player.position;
                dir.y = 0f;
                float dist = dir.magnitude;

                float angle = -Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                arrowRT.localRotation = Quaternion.Euler(0f, 0f, angle);
                distanceText.text = "DISTANCE: " + dist.ToString("F1") + "m";

                if (dist < 3f)
                    directionArrow.color = new Color(0.3f, 1f, 0.5f);
                else if (dist < 8f)
                    directionArrow.color = new Color(1f, 0.8f, 0.3f);
                else
                    directionArrow.color = new Color(1f, 0.5f, 0.2f);
            }
            else
            {
                distanceText.text = "NO TARGETS";
                directionArrow.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }

            if (rocketMovement != null)
                speedText.text = "SPEED: " +
                    rocketMovement.LinearVelocity.magnitude.ToString("F1") + " m/s";
        }

        if (playerCollider != null && player != null)
        {
            float range = playerCollider.radius *
                Mathf.Max(player.lossyScale.x, player.lossyScale.z);
            rangeText.text = "PICKUP RANGE: " + range.ToString("F1") + "m";
        }

        if (notifyTimer > 0f)
        {
            notifyTimer -= Time.deltaTime;
            Color c = notifyText.color;
            notifyText.color = new Color(c.r, c.g, c.b,
                Mathf.Clamp01(notifyTimer / 0.5f));
        }
    }

    Image AddImage(Transform parent, string goName, Color color)
    {
        GameObject go = new GameObject(goName, typeof(RectTransform), typeof(CanvasRenderer));
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    Text AddLabel(Transform parent, string content, int size, Color color,
        TextAnchor align, Vector2 aMin, Vector2 aMax)
    {
        GameObject go = new GameObject(content, typeof(RectTransform), typeof(CanvasRenderer));
        go.transform.SetParent(parent, false);
        Text txt = go.AddComponent<Text>();
        txt.text = content;
        txt.fontSize = size;
        txt.color = color;
        txt.alignment = align;
        txt.fontStyle = FontStyle.Bold;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (txt.font == null)
            txt.font = Font.CreateDynamicFontFromOSFont("Arial", size);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return txt;
    }

    void SetAnchors(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}