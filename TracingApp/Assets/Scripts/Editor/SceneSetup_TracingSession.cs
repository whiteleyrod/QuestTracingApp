using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor script to set up the TracingSession scene.
/// </summary>
public class SceneSetup_TracingSession
{
    [MenuItem("TracingApp/Setup TracingSession Scene")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/TracingSession.unity", OpenSceneMode.Single);

        // --- EventSystem ---
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // --- Main Camera ---
        var cam = GameObject.Find("Main Camera");
        if (cam == null)
        {
            cam = new GameObject("Main Camera");
            cam.tag = "MainCamera";
            var camera = cam.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
        }
        else
        {
            var camera = cam.GetComponent<Camera>();
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
            }
        }

        // --- Pattern Display (World-Space Canvas) ---
        var patternCanvasGO = new GameObject("PatternCanvas");
        var patternCanvas = patternCanvasGO.AddComponent<Canvas>();
        patternCanvas.renderMode = RenderMode.WorldSpace;
        patternCanvasGO.AddComponent<CanvasScaler>();
        patternCanvasGO.AddComponent<GraphicRaycaster>();

        var pcRect = patternCanvasGO.GetComponent<RectTransform>();
        pcRect.sizeDelta = new Vector2(1024, 1024);
        pcRect.localScale = new Vector3(0.002f, 0.002f, 0.002f); // ~2m x 2m in world
        patternCanvasGO.transform.position = new Vector3(0, 1.0f, 2f); // 2m in front, lowered for full visibility

        // Pattern Image (RawImage to display the pattern texture)
        var patternImageGO = new GameObject("PatternImage", typeof(RectTransform), typeof(RawImage));
        patternImageGO.transform.SetParent(patternCanvasGO.transform, false);
        var piRect = patternImageGO.GetComponent<RectTransform>();
        piRect.anchorMin = Vector2.zero;
        piRect.anchorMax = Vector2.one;
        piRect.offsetMin = Vector2.zero;
        piRect.offsetMax = Vector2.zero;
        var patternRawImage = patternImageGO.GetComponent<RawImage>();
        patternRawImage.color = Color.white;

        // Trace Overlay (RawImage on top of pattern for burn trail)
        var traceOverlayGO = new GameObject("TraceOverlay", typeof(RectTransform), typeof(RawImage));
        traceOverlayGO.transform.SetParent(patternCanvasGO.transform, false);
        var toRect = traceOverlayGO.GetComponent<RectTransform>();
        toRect.anchorMin = Vector2.zero;
        toRect.anchorMax = Vector2.one;
        toRect.offsetMin = Vector2.zero;
        toRect.offsetMax = Vector2.zero;
        var traceRawImage = traceOverlayGO.GetComponent<RawImage>();
        traceRawImage.color = new Color(1, 1, 1, 0.8f);

        // Add a collider to the pattern canvas for raycast hit detection
        var patternCollider = patternCanvasGO.AddComponent<BoxCollider>();
        patternCollider.size = new Vector3(1024, 1024, 1);

        // --- Laser Pointer ---
        var laserGO = new GameObject("LaserPointer");
        var lineRenderer = laserGO.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.002f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1, 0, 0, 0.3f);
        lineRenderer.endColor = new Color(1, 0, 0, 0.3f);

        var laserDotGO = new GameObject("LaserDot");
        laserDotGO.transform.SetParent(laserGO.transform, false);
        // Small sphere as dot indicator
        var dotMeshFilter = laserDotGO.AddComponent<MeshFilter>();
        dotMeshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx") ??
            AssetDatabase.LoadAssetAtPath<Mesh>("Library/unity default resources");
        var dotRenderer = laserDotGO.AddComponent<MeshRenderer>();
        var dotMat = new Material(Shader.Find("Sprites/Default"));
        dotMat.color = Color.red;
        dotRenderer.sharedMaterial = dotMat;
        laserDotGO.transform.localScale = Vector3.one * 0.01f;

        var laserPointer = laserGO.AddComponent<TracingApp.Input.LaserPointer>();
        laserPointer.LaserLine = lineRenderer;
        laserPointer.LaserDot = laserDotGO.transform;
        laserPointer.GazeCamera = cam.GetComponent<Camera>();

        // --- Tracing Recorder ---
        var tracingRecorderGO = new GameObject("TracingRecorder");
        var tracingRecorder = tracingRecorderGO.AddComponent<TracingApp.Tracing.TracingRecorder>();
        laserPointer.TracingRecorder = tracingRecorder;

        // --- Pattern Validator ---
        var patternValidatorGO = new GameObject("PatternValidator");
        patternValidatorGO.AddComponent<TracingApp.Tracing.PatternValidator>();

        // --- Managers ---
        var managersGO = new GameObject("Managers");

        var trialManagerGO = new GameObject("TrialManager");
        trialManagerGO.transform.SetParent(managersGO.transform, false);
        var trialManager = trialManagerGO.AddComponent<TracingApp.Core.TrialManager>();
        trialManager.PatternDisplay = patternRawImage;
        trialManager.TracingRecorder = tracingRecorder;

        var sessionManagerGO = new GameObject("SessionManager");
        sessionManagerGO.transform.SetParent(managersGO.transform, false);
        var sessionManager = sessionManagerGO.AddComponent<TracingApp.Core.SessionManager>();
        sessionManager.TrialManager = trialManager;

        // --- Controller Input ---
        var controllerInputGO = new GameObject("ControllerInput");
        var controllerInput = controllerInputGO.AddComponent<TracingApp.Input.ControllerInput>();
        controllerInput.TrialManager = trialManager;
        controllerInput.LaserPointer = laserPointer;

        // --- HUD Canvas (Screen Space) ---
        var hudCanvasGO = new GameObject("HUDCanvas");
        var hudCanvas = hudCanvasGO.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var hudScaler = hudCanvasGO.AddComponent<CanvasScaler>();
        hudScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        hudScaler.referenceResolution = new Vector2(1920, 1080);
        hudCanvasGO.AddComponent<GraphicRaycaster>();

        // Top bar
        var topBar = CreatePanel("TopBar", hudCanvasGO.transform);
        var tbRect = topBar.GetComponent<RectTransform>();
        tbRect.anchorMin = new Vector2(0, 1);
        tbRect.anchorMax = new Vector2(1, 1);
        tbRect.pivot = new Vector2(0.5f, 1);
        tbRect.anchoredPosition = Vector2.zero;
        tbRect.sizeDelta = new Vector2(0, 60);
        topBar.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        var tbLayout = topBar.AddComponent<HorizontalLayoutGroup>();
        tbLayout.spacing = 30;
        tbLayout.padding = new RectOffset(20, 20, 5, 5);
        tbLayout.childAlignment = TextAnchor.MiddleCenter;
        tbLayout.childControlWidth = true;
        tbLayout.childControlHeight = true;
        tbLayout.childForceExpandWidth = true;
        tbLayout.childForceExpandHeight = true;

        var trialCounterText = CreateTMPText("TrialCounterText", topBar.transform, "Pattern 1 of 5", 24);
        trialCounterText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        var trialTimerText = CreateTMPText("TrialTimerText", topBar.transform, "0.0s", 24);
        trialTimerText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        var timeRemainingText = CreateTMPText("TimeRemainingText", topBar.transform, "Time: 05:00", 24);
        timeRemainingText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

        // Recording Indicator (top-right)
        var recIndicatorGO = new GameObject("RecordingIndicator", typeof(RectTransform));
        recIndicatorGO.transform.SetParent(hudCanvasGO.transform, false);
        var riRect = recIndicatorGO.GetComponent<RectTransform>();
        riRect.anchorMin = new Vector2(1, 1);
        riRect.anchorMax = new Vector2(1, 1);
        riRect.pivot = new Vector2(1, 1);
        riRect.anchoredPosition = new Vector2(-20, -70);
        riRect.sizeDelta = new Vector2(200, 40);

        var recLayout = recIndicatorGO.AddComponent<HorizontalLayoutGroup>();
        recLayout.spacing = 8;
        recLayout.childAlignment = TextAnchor.MiddleRight;
        recLayout.childControlWidth = false;
        recLayout.childControlHeight = true;
        recLayout.childForceExpandWidth = false;
        recLayout.childForceExpandHeight = true;

        var recDot = new GameObject("RecDot", typeof(RectTransform), typeof(Image));
        recDot.transform.SetParent(recIndicatorGO.transform, false);
        recDot.GetComponent<Image>().color = Color.red;
        recDot.AddComponent<LayoutElement>().preferredWidth = 20;
        // Make it circular-ish (will be square but red)

        var recLabel = CreateTMPText("RecLabel", recIndicatorGO.transform, "REC", 20);
        recLabel.GetComponent<TextMeshProUGUI>().color = Color.red;
        recLabel.AddComponent<LayoutElement>().preferredWidth = 50;

        var recIndicator = recIndicatorGO.AddComponent<TracingApp.UI.RecordingIndicator>();
        recIndicator.IndicatorImage = recDot.GetComponent<Image>();
        recIndicator.RecordingLabel = recLabel.GetComponent<TextMeshProUGUI>();

        // Results Panel (hidden by default, shown after trial)
        var resultsPanel = CreatePanel("ResultsPanel", hudCanvasGO.transform);
        var rpRect = resultsPanel.GetComponent<RectTransform>();
        rpRect.anchorMin = new Vector2(0.5f, 0.5f);
        rpRect.anchorMax = new Vector2(0.5f, 0.5f);
        rpRect.pivot = new Vector2(0.5f, 0.5f);
        rpRect.sizeDelta = new Vector2(500, 350);
        resultsPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        var rpLayout = resultsPanel.AddComponent<VerticalLayoutGroup>();
        rpLayout.spacing = 12;
        rpLayout.padding = new RectOffset(30, 30, 25, 25);
        rpLayout.childAlignment = TextAnchor.MiddleCenter;
        rpLayout.childControlWidth = true;
        rpLayout.childControlHeight = false;
        rpLayout.childForceExpandWidth = true;
        rpLayout.childForceExpandHeight = false;

        var rpTitle = CreateTMPText("ResultsTitle", resultsPanel.transform, "Trial Results", 32);
        rpTitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rpTitle.AddComponent<LayoutElement>().preferredHeight = 45;

        var rpPatternName = CreateTMPText("PatternNameText", resultsPanel.transform, "", 22);
        rpPatternName.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rpPatternName.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
        rpPatternName.AddComponent<LayoutElement>().preferredHeight = 30;

        var rpTime = CreateTMPText("TimeText", resultsPanel.transform, "Time: 0.0 seconds", 26);
        rpTime.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rpTime.AddComponent<LayoutElement>().preferredHeight = 35;

        var rpAccuracy = CreateTMPText("AccuracyText", resultsPanel.transform, "Accuracy: 0.0% Correct", 26);
        rpAccuracy.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rpAccuracy.AddComponent<LayoutElement>().preferredHeight = 35;

        var rpCoverage = CreateTMPText("CoverageText", resultsPanel.transform, "Coverage: 0.0% Completed", 26);
        rpCoverage.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rpCoverage.AddComponent<LayoutElement>().preferredHeight = 35;

        var rpCountdown = CreateTMPText("CountdownText", resultsPanel.transform, "Next in 10s", 18);
        rpCountdown.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rpCountdown.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
        rpCountdown.AddComponent<LayoutElement>().preferredHeight = 25;

        var rpNextBtn = CreateTMPButton("NextButton", resultsPanel.transform, "Next →", new Color(0.2f, 0.5f, 0.7f, 1f));
        rpNextBtn.AddComponent<LayoutElement>().preferredHeight = 45;

        // Results Display component
        var resultsDisplay = hudCanvasGO.AddComponent<TracingApp.UI.ResultsDisplay>();
        resultsDisplay.ResultsPanel = resultsPanel;
        resultsDisplay.TimeText = rpTime.GetComponent<TextMeshProUGUI>();
        resultsDisplay.AccuracyText = rpAccuracy.GetComponent<TextMeshProUGUI>();
        resultsDisplay.CoverageText = rpCoverage.GetComponent<TextMeshProUGUI>();
        resultsDisplay.PatternNameText = rpPatternName.GetComponent<TextMeshProUGUI>();
        resultsDisplay.CountdownText = rpCountdown.GetComponent<TextMeshProUGUI>();
        resultsDisplay.NextButton = rpNextBtn.GetComponent<Button>();

        // Trial Counter text reference
        var trialCounterRef = CreateTMPText("TrialCounterRef", resultsPanel.transform, "", 0);
        trialCounterRef.SetActive(false);

        // Session HUD component
        var sessionHUD = hudCanvasGO.AddComponent<TracingApp.UI.SessionHUD>();
        sessionHUD.TrialCounterText = trialCounterText.GetComponent<TextMeshProUGUI>();
        sessionHUD.TimeRemainingText = timeRemainingText.GetComponent<TextMeshProUGUI>();
        sessionHUD.TrialTimerText = trialTimerText.GetComponent<TextMeshProUGUI>();
        sessionHUD.RecordingIndicator = recIndicator;
        sessionHUD.ResultsDisplay = resultsDisplay;

        // Instruction text at bottom
        var instructionText = CreateTMPText("InstructionText", hudCanvasGO.transform, "Press SPACE or click to start/stop tracing", 20);
        var itRect = instructionText.GetComponent<RectTransform>();
        itRect.anchorMin = new Vector2(0.5f, 0);
        itRect.anchorMax = new Vector2(0.5f, 0);
        itRect.pivot = new Vector2(0.5f, 0);
        itRect.anchoredPosition = new Vector2(0, 20);
        itRect.sizeDelta = new Vector2(600, 40);
        instructionText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        instructionText.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] TracingSession scene setup complete!");
    }

    // --- Helper Methods ---

    static GameObject CreatePanel(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        return go;
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
        tmp.fontSize = 22;
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
