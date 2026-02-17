using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor script to set up the Results scene with session summary UI.
/// </summary>
public class SceneSetup_Results
{
    [MenuItem("TracingApp/Setup Results Scene")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Results.unity", OpenSceneMode.Single);

        // --- EventSystem ---
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // --- UI Canvas ---
        var canvasGO = new GameObject("ResultsCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Background ---
        var bg = CreatePanel("Background", canvasGO.transform);
        SetStretchAll(bg);
        bg.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 1f);

        // --- Title ---
        var title = CreateTMPText("TitleText", canvasGO.transform, "Session Summary", 48);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -25);
        titleRect.sizeDelta = new Vector2(800, 65);
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        // --- Stats Panel (top area) ---
        var statsPanel = CreatePanel("StatsPanel", canvasGO.transform);
        var spRect = statsPanel.GetComponent<RectTransform>();
        spRect.anchorMin = new Vector2(0.05f, 0.7f);
        spRect.anchorMax = new Vector2(0.95f, 0.9f);
        spRect.offsetMin = Vector2.zero;
        spRect.offsetMax = Vector2.zero;
        statsPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        var spLayout = statsPanel.AddComponent<HorizontalLayoutGroup>();
        spLayout.spacing = 20;
        spLayout.padding = new RectOffset(30, 30, 15, 15);
        spLayout.childAlignment = TextAnchor.MiddleCenter;
        spLayout.childControlWidth = true;
        spLayout.childControlHeight = true;
        spLayout.childForceExpandWidth = true;
        spLayout.childForceExpandHeight = true;

        var overallScore = CreateTMPText("OverallScoreText", statsPanel.transform, "Overall: 0.0%", 28);
        overallScore.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        overallScore.GetComponent<TextMeshProUGUI>().color = new Color(0.3f, 0.8f, 0.3f);

        var avgAccuracy = CreateTMPText("AvgAccuracyText", statsPanel.transform, "Avg Accuracy: 0.0%", 22);
        avgAccuracy.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        var avgCoverage = CreateTMPText("AvgCoverageText", statsPanel.transform, "Avg Coverage: 0.0%", 22);
        avgCoverage.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        var avgTime = CreateTMPText("AvgTimeText", statsPanel.transform, "Avg Time: 0.0s", 22);
        avgTime.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        var totalTime = CreateTMPText("TotalTimeText", statsPanel.transform, "Total: 0.0s", 22);
        totalTime.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        var totalTrials = CreateTMPText("TotalTrialsText", statsPanel.transform, "Trials: 0", 22);
        totalTrials.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        // --- Trial List Header ---
        var headerPanel = CreatePanel("HeaderPanel", canvasGO.transform);
        var hpRect = headerPanel.GetComponent<RectTransform>();
        hpRect.anchorMin = new Vector2(0.05f, 0.63f);
        hpRect.anchorMax = new Vector2(0.95f, 0.69f);
        hpRect.offsetMin = Vector2.zero;
        hpRect.offsetMax = Vector2.zero;
        headerPanel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.3f, 1f);

        var hLayout = headerPanel.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 10;
        hLayout.padding = new RectOffset(20, 20, 5, 5);
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = true;

        CreateTMPText("H_Pattern", headerPanel.transform, "Pattern", 20).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("H_Time", headerPanel.transform, "Time", 20).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("H_Accuracy", headerPanel.transform, "Accuracy", 20).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("H_Coverage", headerPanel.transform, "Coverage", 20).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // --- Trial List (ScrollView area) ---
        var scrollPanel = CreatePanel("TrialListPanel", canvasGO.transform);
        var slRect = scrollPanel.GetComponent<RectTransform>();
        slRect.anchorMin = new Vector2(0.05f, 0.15f);
        slRect.anchorMax = new Vector2(0.95f, 0.63f);
        slRect.offsetMin = Vector2.zero;
        slRect.offsetMax = Vector2.zero;
        scrollPanel.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.17f, 0.9f);

        var scrollRect = scrollPanel.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        // Viewport
        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollPanel.transform, false);
        viewport.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f); // Nearly transparent for mask
        viewport.GetComponent<Mask>().showMaskGraphic = false;
        var vpRect = viewport.GetComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero;
        vpRect.anchorMax = Vector2.one;
        vpRect.offsetMin = new Vector2(10, 10);
        vpRect.offsetMax = new Vector2(-10, -10);
        scrollRect.viewport = vpRect;

        // Content container
        var content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0);
        scrollRect.content = contentRect;

        var contentLayout = content.AddComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 5;
        contentLayout.padding = new RectOffset(10, 10, 5, 5);
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;

        var contentFitter = content.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // --- Trial Row Prefab (create in scene, will be used as template) ---
        var trialRowPrefab = CreateTrialRowTemplate("TrialRowTemplate", canvasGO.transform);
        trialRowPrefab.SetActive(false); // Hidden template

        // --- Bottom Buttons ---
        var bottomPanel = CreatePanel("BottomPanel", canvasGO.transform);
        var bpRect = bottomPanel.GetComponent<RectTransform>();
        bpRect.anchorMin = new Vector2(0.1f, 0f);
        bpRect.anchorMax = new Vector2(0.9f, 0f);
        bpRect.pivot = new Vector2(0.5f, 0f);
        bpRect.anchoredPosition = new Vector2(0, 20);
        bpRect.sizeDelta = new Vector2(0, 60);
        bottomPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        var bpLayout = bottomPanel.AddComponent<HorizontalLayoutGroup>();
        bpLayout.spacing = 30;
        bpLayout.childAlignment = TextAnchor.MiddleCenter;
        bpLayout.childControlWidth = true;
        bpLayout.childControlHeight = true;
        bpLayout.childForceExpandWidth = true;
        bpLayout.childForceExpandHeight = true;

        var newSessionBtn = CreateTMPButton("NewSessionButton", bottomPanel.transform, "New Session", new Color(0.2f, 0.5f, 0.7f, 1f));
        var mainMenuBtn = CreateTMPButton("MainMenuButton", bottomPanel.transform, "Main Menu", new Color(0.4f, 0.4f, 0.5f, 1f));
        var exitBtn = CreateTMPButton("ExitButton", bottomPanel.transform, "Exit", new Color(0.6f, 0.2f, 0.2f, 1f));

        // --- Wire up SessionSummary component ---
        var summary = canvasGO.AddComponent<TracingApp.UI.SessionSummary>();
        summary.TitleText = title.GetComponent<TextMeshProUGUI>();
        summary.OverallScoreText = overallScore.GetComponent<TextMeshProUGUI>();
        summary.AvgAccuracyText = avgAccuracy.GetComponent<TextMeshProUGUI>();
        summary.AvgCoverageText = avgCoverage.GetComponent<TextMeshProUGUI>();
        summary.AvgTimeText = avgTime.GetComponent<TextMeshProUGUI>();
        summary.TotalTimeText = totalTime.GetComponent<TextMeshProUGUI>();
        summary.TotalTrialsText = totalTrials.GetComponent<TextMeshProUGUI>();
        summary.TrialListContainer = content.transform;
        summary.TrialRowPrefab = trialRowPrefab;
        summary.NewSessionButton = newSessionBtn.GetComponent<Button>();
        summary.MainMenuButton = mainMenuBtn.GetComponent<Button>();
        summary.ExitButton = exitBtn.GetComponent<Button>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] Results scene setup complete!");
    }

    // --- Helper Methods ---

    static GameObject CreateTrialRowTemplate(string name, Transform parent)
    {
        var row = new GameObject(name, typeof(RectTransform), typeof(Image));
        row.transform.SetParent(parent, false);
        row.GetComponent<Image>().color = new Color(0.18f, 0.18f, 0.23f, 0.8f);
        row.AddComponent<LayoutElement>().preferredHeight = 40;

        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(15, 15, 5, 5);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        CreateTMPText("PatternName", row.transform, "Pattern", 18);
        CreateTMPText("Time", row.transform, "0.0s", 18);
        CreateTMPText("Accuracy", row.transform, "0.0%", 18);
        CreateTMPText("Coverage", row.transform, "0.0%", 18);

        return row;
    }

    static GameObject CreatePanel(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void SetStretchAll(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static GameObject CreateTMPText(string name, Transform parent, string text, int fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        return go;
    }

    static GameObject CreateTMPButton(string name, Transform parent, string label, Color bgColor)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = bgColor;

        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return go;
    }
}
