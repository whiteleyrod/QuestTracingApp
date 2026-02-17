using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor script to set up the Settings scene.
/// </summary>
public class SceneSetup_Settings
{
    [MenuItem("TracingApp/Setup Settings Scene")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Settings.unity", OpenSceneMode.Single);

        // Clean up existing objects (except camera/light)
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go != null && go.name != "Main Camera" && go.name != "Directional Light" && go.transform.parent == null)
                toDestroy.Add(go);
        }
        foreach (var go in toDestroy)
        {
            if (go != null) Object.DestroyImmediate(go);
        }

        // --- EventSystem ---
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // --- UI Canvas ---
        var canvasGO = new GameObject("SettingsCanvas");
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
        var title = CreateTMPText("Title", canvasGO.transform, "Session Settings", 48);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -30);
        titleRect.sizeDelta = new Vector2(800, 70);
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        // --- Two-column layout ---
        var columnsPanel = CreatePanel("ColumnsPanel", canvasGO.transform);
        var colRect = columnsPanel.GetComponent<RectTransform>();
        colRect.anchorMin = new Vector2(0.05f, 0.15f);
        colRect.anchorMax = new Vector2(0.95f, 0.88f);
        colRect.offsetMin = Vector2.zero;
        colRect.offsetMax = Vector2.zero;
        columnsPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        var colLayout = columnsPanel.AddComponent<HorizontalLayoutGroup>();
        colLayout.spacing = 30;
        colLayout.childControlWidth = true;
        colLayout.childControlHeight = true;
        colLayout.childForceExpandWidth = true;
        colLayout.childForceExpandHeight = true;

        // ========== LEFT COLUMN: Session Settings ==========
        var leftCol = CreatePanel("SessionSettingsPanel", columnsPanel.transform);
        leftCol.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        var leftLayout = leftCol.AddComponent<VerticalLayoutGroup>();
        leftLayout.spacing = 20;
        leftLayout.padding = new RectOffset(30, 30, 25, 25);
        leftLayout.childAlignment = TextAnchor.UpperCenter;
        leftLayout.childControlWidth = true;
        leftLayout.childControlHeight = false;
        leftLayout.childForceExpandWidth = true;
        leftLayout.childForceExpandHeight = false;

        // Section Title
        var sessTitle = CreateTMPText("SectionTitle", leftCol.transform, "How long to train?", 30);
        sessTitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        sessTitle.AddComponent<LayoutElement>().preferredHeight = 45;

        // Time Limit Row
        var timeRow = CreateUpDownRow("TimeLimitRow", leftCol.transform, "Time Limit:");
        var timeLimitText = timeRow.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
        timeLimitText.text = "5 min";
        var timeUpBtn = timeRow.transform.Find("UpButton").gameObject;
        var timeDownBtn = timeRow.transform.Find("DownButton").gameObject;

        // Pattern Count Row
        var countRow = CreateUpDownRow("PatternCountRow", leftCol.transform, "Patterns:");
        var patternCountText = countRow.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
        patternCountText.text = "5";
        var countUpBtn = countRow.transform.Find("UpButton").gameObject;
        var countDownBtn = countRow.transform.Find("DownButton").gameObject;

        // Spacer
        var spacer1 = new GameObject("Spacer", typeof(RectTransform));
        spacer1.transform.SetParent(leftCol.transform, false);
        spacer1.AddComponent<LayoutElement>().flexibleHeight = 1;

        // Randomize Patterns Button
        var randPatternsBtn = CreateTMPButton("RandomizePatternsButton", leftCol.transform,
            "Surprise me (Randomise)", new Color(0.3f, 0.3f, 0.6f, 1f));
        randPatternsBtn.AddComponent<LayoutElement>().preferredHeight = 55;

        // ========== RIGHT COLUMN: Level Settings ==========
        var rightCol = CreatePanel("LevelSettingsPanel", columnsPanel.transform);
        rightCol.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        var rightLayout = rightCol.AddComponent<VerticalLayoutGroup>();
        rightLayout.spacing = 12;
        rightLayout.padding = new RectOffset(30, 30, 25, 25);
        rightLayout.childAlignment = TextAnchor.UpperCenter;
        rightLayout.childControlWidth = true;
        rightLayout.childControlHeight = false;
        rightLayout.childForceExpandWidth = true;
        rightLayout.childForceExpandHeight = false;

        // Section Title
        var lvlTitle = CreateTMPText("SectionTitle", rightCol.transform, "Difficulty Levels", 30);
        lvlTitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        lvlTitle.AddComponent<LayoutElement>().preferredHeight = 45;

        // Difficulty Preset Row
        var presetButtons = CreatePresetRow(rightCol.transform);

        // Complexity Slider
        var complexitySlider = CreateLabeledSlider("ComplexityRow", rightCol.transform, "Complexity:", "Simple (1)", 1, 5);
        var complexityLabel = complexitySlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        // Thickness Slider
        var thicknessSlider = CreateLabeledSlider("ThicknessRow", rightCol.transform, "Line Thickness:", "Thin (10px)", 1, 5);
        var thicknessLabel = thicknessSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        // Scale Slider
        var scaleSlider = CreateLabeledSlider("ScaleRow", rightCol.transform, "Scale:", "512x512", 1, 10);
        var scaleLabel = scaleSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        // Spacer
        var spacer2 = new GameObject("Spacer", typeof(RectTransform));
        spacer2.transform.SetParent(rightCol.transform, false);
        spacer2.AddComponent<LayoutElement>().flexibleHeight = 1;

        // Randomize Levels Button
        var randLevelsBtn = CreateTMPButton("RandomizeLevelsButton", rightCol.transform,
            "Surprise me (Randomise)", new Color(0.3f, 0.3f, 0.6f, 1f));
        randLevelsBtn.AddComponent<LayoutElement>().preferredHeight = 55;

        // ========== Bottom Buttons ==========
        var bottomPanel = CreatePanel("BottomPanel", canvasGO.transform);
        var bpRect = bottomPanel.GetComponent<RectTransform>();
        bpRect.anchorMin = new Vector2(0.2f, 0f);
        bpRect.anchorMax = new Vector2(0.8f, 0f);
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

        var backBtn = CreateTMPButton("BackButton", bottomPanel.transform, "Back", new Color(0.4f, 0.4f, 0.5f, 1f));
        var startBtn = CreateTMPButton("StartButton", bottomPanel.transform, "Start Session", new Color(0.2f, 0.6f, 0.2f, 1f));

        // ========== Wire up SettingsController ==========
        var settingsCtrl = canvasGO.AddComponent<TracingApp.UI.SettingsController>();
        settingsCtrl.TimeLimitText = timeLimitText;
        settingsCtrl.TimeLimitUpButton = timeUpBtn.GetComponent<Button>();
        settingsCtrl.TimeLimitDownButton = timeDownBtn.GetComponent<Button>();
        settingsCtrl.PatternCountText = patternCountText;
        settingsCtrl.PatternCountUpButton = countUpBtn.GetComponent<Button>();
        settingsCtrl.PatternCountDownButton = countDownBtn.GetComponent<Button>();
        settingsCtrl.RandomizePatternsButton = randPatternsBtn.GetComponent<Button>();
        settingsCtrl.ComplexitySlider = complexitySlider.transform.Find("Slider").GetComponent<Slider>();
        settingsCtrl.ComplexityLabel = complexityLabel;
        settingsCtrl.ThicknessSlider = thicknessSlider.transform.Find("Slider").GetComponent<Slider>();
        settingsCtrl.ThicknessLabel = thicknessLabel;
        settingsCtrl.ScaleSlider = scaleSlider.transform.Find("Slider").GetComponent<Slider>();
        settingsCtrl.ScaleLabel = scaleLabel;
        settingsCtrl.PresetButtons = presetButtons;
        settingsCtrl.RandomizeLevelsButton = randLevelsBtn.GetComponent<Button>();
        settingsCtrl.StartButton = startBtn.GetComponent<Button>();
        settingsCtrl.BackButton = backBtn.GetComponent<Button>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] Settings scene setup complete!");
    }

    // ==================== HELPER METHODS ====================

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

    static Button[] CreatePresetRow(Transform parent)
    {
        var presetNames = new[]
        {
            "I'm new here",
            "Getting the hang",
            "Steady hands",
            "Bring it on",
            "Woah!"
        };

        var buttons = new Button[presetNames.Length];

        var row = new GameObject("DifficultyPresetRow", typeof(RectTransform));
        row.transform.SetParent(parent, false);
        row.AddComponent<LayoutElement>().preferredHeight = 56;

        var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 8;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlWidth = false;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = false;

        for (int i = 0; i < presetNames.Length; i++)
        {
            var btn = CreateTMPButton($"PresetButton{i + 1}", row.transform, presetNames[i], new Color(0.3f, 0.3f, 0.4f, 1f));
            var btnLayout = btn.AddComponent<LayoutElement>();
            btnLayout.preferredWidth = 145;
            btnLayout.preferredHeight = 46;
            btn.transform.Find("Text").GetComponent<TextMeshProUGUI>().fontSize = 16;
            buttons[i] = btn.GetComponent<Button>();
        }

        return buttons;
    }

    /// <summary>
    /// Creates a row with Label | [−] | Value | [+] layout for up/down controls.
    /// </summary>
    static GameObject CreateUpDownRow(string name, Transform parent, string label)
    {
        var row = new GameObject(name, typeof(RectTransform));
        row.transform.SetParent(parent, false);
        row.AddComponent<LayoutElement>().preferredHeight = 55;

        var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 12;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlWidth = false;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = true;

        // Label
        var labelGO = CreateTMPText("Label", row.transform, label, 24);
        labelGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        labelGO.AddComponent<LayoutElement>().preferredWidth = 160;

        // Down Button
        var downBtn = new GameObject("DownButton", typeof(RectTransform), typeof(Image), typeof(Button));
        downBtn.transform.SetParent(row.transform, false);
        downBtn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.4f, 1f);
        downBtn.AddComponent<LayoutElement>().preferredWidth = 50;
        var downText = CreateTMPText("Text", downBtn.transform, "−", 30);
        downText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        SetStretchAll(downText);

        // Value Text
        var valueGO = CreateTMPText("ValueText", row.transform, "5", 26);
        valueGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        valueGO.AddComponent<LayoutElement>().preferredWidth = 100;

        // Up Button
        var upBtn = new GameObject("UpButton", typeof(RectTransform), typeof(Image), typeof(Button));
        upBtn.transform.SetParent(row.transform, false);
        upBtn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.4f, 1f);
        upBtn.AddComponent<LayoutElement>().preferredWidth = 50;
        var upText = CreateTMPText("Text", upBtn.transform, "+", 30);
        upText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        SetStretchAll(upText);

        return row;
    }

    /// <summary>
    /// Creates a labeled slider row with: "Label ......... Value" on top, slider bar below.
    /// "ValueLabel" is a direct child of the container for SettingsController to find via transform.Find("ValueLabel").
    /// </summary>
    static GameObject CreateLabeledSlider(string name, Transform parent, string label, string defaultValue, float min, float max)
    {
        // Outer container participates in parent vertical layout
        var container = new GameObject(name, typeof(RectTransform));
        container.transform.SetParent(parent, false);
        container.AddComponent<LayoutElement>().preferredHeight = 70;

        // We'll manually position children using anchors instead of a nested VerticalLayoutGroup
        // to avoid layout conflicts.

        // Label text (top-left)
        var labelGO = CreateTMPText("Label", container.transform, label, 22);
        labelGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        var labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.55f);
        labelRect.anchorMax = new Vector2(0.6f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        // ValueLabel (top-right) — this is the one SettingsController writes to
        var valueLabel = CreateTMPText("ValueLabel", container.transform, defaultValue, 22);
        valueLabel.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.8f, 1f);
        valueLabel.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        var vlRect = valueLabel.GetComponent<RectTransform>();
        vlRect.anchorMin = new Vector2(0.4f, 0.55f);
        vlRect.anchorMax = new Vector2(1f, 1f);
        vlRect.offsetMin = Vector2.zero;
        vlRect.offsetMax = Vector2.zero;

        // Slider (bottom half)
        var sliderGO = BuildSlider("Slider", container.transform, min, max);
        var sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0, 0);
        sliderRect.anchorMax = new Vector2(1, 0.45f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        return container;
    }

    /// <summary>
    /// Builds a Unity UI Slider with background, fill, and handle.
    /// </summary>
    static GameObject BuildSlider(string name, Transform parent, float min, float max)
    {
        var sliderGO = new GameObject(name, typeof(RectTransform), typeof(Slider));
        sliderGO.transform.SetParent(parent, false);

        // Background
        var bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bgGO.transform.SetParent(sliderGO.transform, false);
        bgGO.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f, 1f);
        var bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.25f);
        bgRect.anchorMax = new Vector2(1, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill Area
        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGO.transform, false);
        var faRect = fillArea.GetComponent<RectTransform>();
        faRect.anchorMin = new Vector2(0, 0.25f);
        faRect.anchorMax = new Vector2(1, 0.75f);
        faRect.offsetMin = new Vector2(5, 0);
        faRect.offsetMax = new Vector2(-5, 0);

        var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        fill.GetComponent<Image>().color = new Color(0.3f, 0.5f, 0.8f, 1f);
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Handle Slide Area
        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderGO.transform, false);
        var haRect = handleArea.GetComponent<RectTransform>();
        haRect.anchorMin = Vector2.zero;
        haRect.anchorMax = Vector2.one;
        haRect.offsetMin = new Vector2(10, 0);
        haRect.offsetMax = new Vector2(-10, 0);

        var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        handle.GetComponent<Image>().color = Color.white;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);

        // Configure slider component
        var slider = sliderGO.GetComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = true;
        slider.value = min;

        return sliderGO;
    }
}
