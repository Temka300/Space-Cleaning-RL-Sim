using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Display")]
    public int targetDisplay = 1;

    private EnvironmentManager env;
    private Text remainingText;
    private Text collectedText;
    private Text timerText;
    private Text statusText;
    private float episodeStartTime;
    private int lastRemaining = -1;
    private int totalDebris;

    void Start()
    {
        env = FindFirstObjectByType<EnvironmentManager>();
        BuildCanvas();
        episodeStartTime = Time.time;
    }

    void BuildCanvas()
    {
        GameObject canvasGO = new GameObject("HUD_Canvas");
        canvasGO.transform.SetParent(transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.targetDisplay = targetDisplay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        Image bg = AddImage(canvasGO.transform, "BG", new Color(0.08f, 0.08f, 0.1f, 0.9f));
        Stretch(bg.rectTransform);

        AddLabel(canvasGO.transform, "SPACE CLEANING SIM", 42, Color.white,
            TextAnchor.UpperCenter, new Vector2(0f, 0.92f), new Vector2(1f, 1f));

        remainingText = AddLabel(canvasGO.transform, "REMAINING: -", 34,
            new Color(0.4f, 0.9f, 1f),
            TextAnchor.UpperLeft, new Vector2(0.04f, 0.78f), new Vector2(0.5f, 0.88f));

        collectedText = AddLabel(canvasGO.transform, "COLLECTED: 0/0", 34,
            new Color(1f, 0.85f, 0.3f),
            TextAnchor.UpperLeft, new Vector2(0.04f, 0.68f), new Vector2(0.5f, 0.78f));

        timerText = AddLabel(canvasGO.transform, "TIME: 00:00", 34,
            new Color(0.9f, 0.9f, 0.9f),
            TextAnchor.UpperRight, new Vector2(0.5f, 0.78f), new Vector2(0.96f, 0.88f));

        statusText = AddLabel(canvasGO.transform, "COLLECTING...", 40,
            new Color(0.4f, 0.9f, 1f),
            TextAnchor.LowerCenter, new Vector2(0.1f, 0.03f), new Vector2(0.9f, 0.12f));
    }

    void Update()
    {
        if (env == null) return;

        int remaining = env.RemainingDebris;
        int total = env.debrisCount;

        if (remaining > lastRemaining && lastRemaining >= 0)
        {
            totalDebris = total;
            episodeStartTime = Time.time;
        }

        if (totalDebris == 0) totalDebris = total;
        int collected = Mathf.Max(0, totalDebris - remaining);
        lastRemaining = remaining;

        remainingText.text = "REMAINING: " + remaining;
        collectedText.text = "COLLECTED: " + collected + " / " + totalDebris;

        float elapsed = Time.time - episodeStartTime;
        timerText.text = string.Format("TIME: {0:00}:{1:00}",
            (int)(elapsed / 60f), (int)(elapsed % 60f));

        if (remaining <= 0 && totalDebris > 0)
        {
            statusText.text = "ALL CLEAR!";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = "COLLECTING...";
            statusText.color = new Color(0.4f, 0.9f, 1f);
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

    void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}