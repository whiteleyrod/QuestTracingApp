using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor script to set up the MainMenu scene with all required GameObjects and UI.
/// </summary>
public class SceneSetup_MainMenu
{
    [MenuItem("TracingApp/Setup MainMenu Scene")]
    public static void Execute()
    {
        // Open the MainMenu scene
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);

        // --- GameManager (persistent singleton) ---
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<TracingApp.Core.GameManager>();

        // --- EventSystem ---
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // --- Main Camera ---
        // Already exists from scene creation, find it
        var cam = GameObject.Find("Main Camera");
        if (cam == null)
        {
            cam = new GameObject("Main Camera");
            cam.tag = "MainCamera";
            cam.AddComponent<Camera>();
        }

        // --- UI Canvas (Screen Space) ---
        var canvasGO = new GameObject("MainMenuCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Background Panel ---
        var bgPanel = CreatePanel("Background", canvasGO.transform);
        SetStretchAll(bgPanel);
        bgPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 1f);

        // --- Title ---
        var title = CreateTMPText("Title", canvasGO.transform, "VR Tracing App", 64);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -60);
        titleRect.sizeDelta = new Vector2(800, 100);
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        title.GetComponent<TextMeshProUGUI>().color = Color.white;

        // --- Subtitle ---
        var subtitle = CreateTMPText("Subtitle", canvasGO.transform, "Fine Motor Control Assessment", 28);
        var subRect = subtitle.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 1f);
        subRect.anchorMax = new Vector2(0.5f, 1f);
        subRect.pivot = new Vector2(0.5f, 1f);
        subRect.anchoredPosition = new Vector2(0, -170);
        subRect.sizeDelta = new Vector2(800, 50);
        subtitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        subtitle.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f, 1f);

        // --- Center Panel (for user ID entry) ---
        var centerPanel = CreatePanel("CenterPanel", canvasGO.transform);
        var cpRect = centerPanel.GetComponent<RectTransform>();
        cpRect.anchorMin = new Vector2(0.5f, 0.5f);
        cpRect.anchorMax = new Vector2(0.5f, 0.5f);
        cpRect.pivot = new Vector2(0.5f, 0.5f);
        cpRect.anchoredPosition = new Vector2(0, 20);
        cpRect.sizeDelta = new Vector2(600, 350);
        centerPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        var cpLayout = centerPanel.AddComponent<VerticalLayoutGroup>();
        cpLayout.spacing = 15;
        cpLayout.padding = new RectOffset(40, 40, 30, 30);
        cpLayout.childAlignment = TextAnchor.MiddleCenter;
        cpLayout.childControlWidth = true;
        cpLayout.childControlHeight = false;
        cpLayout.childForceExpandWidth = true;
        cpLayout.childForceExpandHeight = false;

        // User ID Label
        var idLabel = CreateTMPText("UserIDLabel", centerPanel.transform, "Enter User ID (3 letters + 3 numbers)", 24);
        idLabel.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        idLabel.GetComponent<TextMeshProUGUI>().color = Color.white;
        var idLabelLE = idLabel.AddComponent<LayoutElement>();
        idLabelLE.preferredHeight = 40;

        // Input Field
        var inputFieldGO = CreateTMPInputField("UserIDInput", centerPanel.transform, "e.g. ABC123");
        var inputLE = inputFieldGO.AddComponent<LayoutElement>();
        inputLE.preferredHeight = 50;

        // Error Text
        var errorText = CreateTMPText("ErrorText", centerPanel.transform, "", 18);
        errorText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.3f, 0.3f, 1f);
        errorText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        var errorLE = errorText.AddComponent<LayoutElement>();
        errorLE.preferredHeight = 30;
        errorText.SetActive(false);

        // Submit Button
        var submitBtn = CreateTMPButton("SubmitButton", centerPanel.transform, "Submit", new Color(0.2f, 0.6f, 0.2f, 1f));
        var submitLE = submitBtn.AddComponent<LayoutElement>();
        submitLE.preferredHeight = 50;

        // Skip Button
        var skipBtn = CreateTMPButton("SkipButton", centerPanel.transform, "Skip (Anonymous)", new Color(0.4f, 0.4f, 0.5f, 1f));
        var skipLE = skipBtn.AddComponent<LayoutElement>();
        skipLE.preferredHeight = 50;

        // --- Bottom Buttons ---
        var bottomPanel = CreatePanel("BottomPanel", canvasGO.transform);
        var bpRect = bottomPanel.GetComponent<RectTransform>();
        bpRect.anchorMin = new Vector2(0.5f, 0f);
        bpRect.anchorMax = new Vector2(0.5f, 0f);
        bpRect.pivot = new Vector2(0.5f, 0f);
        bpRect.anchoredPosition = new Vector2(0, 40);
        bpRect.sizeDelta = new Vector2(400, 60);
        bottomPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        var bpLayout = bottomPanel.AddComponent<HorizontalLayoutGroup>();
        bpLayout.spacing = 20;
        bpLayout.childAlignment = TextAnchor.MiddleCenter;
        bpLayout.childControlWidth = true;
        bpLayout.childControlHeight = true;
        bpLayout.childForceExpandWidth = true;
        bpLayout.childForceExpandHeight = true;

        var exitBtn = CreateTMPButton("ExitButton", bottomPanel.transform, "Exit", new Color(0.6f, 0.2f, 0.2f, 1f));

        // --- Add UserIDEntry component ---
        var userIDEntry = canvasGO.AddComponent<TracingApp.UI.UserIDEntry>();
        userIDEntry.UserIDInput = inputFieldGO.GetComponent<TMP_InputField>();
        userIDEntry.SubmitButton = submitBtn.GetComponent<Button>();
        userIDEntry.SkipButton = skipBtn.GetComponent<Button>();
        userIDEntry.ErrorText = errorText.GetComponent<TextMeshProUGUI>();
        userIDEntry.TitleText = title.GetComponent<TextMeshProUGUI>();

        // Save
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] MainMenu scene setup complete!");
    }

    // --- Helper Methods ---

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

    static GameObject CreateTMPInputField(string name, Transform parent, string placeholder)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);

        // Text Area
        var textArea = new GameObject("Text Area", typeof(RectTransform));
        textArea.transform.SetParent(go.transform, false);
        var taRect = textArea.GetComponent<RectTransform>();
        taRect.anchorMin = Vector2.zero;
        taRect.anchorMax = Vector2.one;
        taRect.offsetMin = new Vector2(10, 6);
        taRect.offsetMax = new Vector2(-10, -7);
        textArea.AddComponent<RectMask2D>();

        // Placeholder
        var phGO = new GameObject("Placeholder", typeof(RectTransform));
        phGO.transform.SetParent(textArea.transform, false);
        var phTMP = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text = placeholder;
        phTMP.fontSize = 24;
        phTMP.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        phTMP.fontStyle = FontStyles.Italic;
        var phRect = phGO.GetComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        phRect.offsetMin = Vector2.zero;
        phRect.offsetMax = Vector2.zero;

        // Text
        var txtGO = new GameObject("Text", typeof(RectTransform));
        txtGO.transform.SetParent(textArea.transform, false);
        var txtTMP = txtGO.AddComponent<TextMeshProUGUI>();
        txtTMP.fontSize = 24;
        txtTMP.color = Color.white;
        var txtRect = txtGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        // Input Field component
        var inputField = go.AddComponent<TMP_InputField>();
        inputField.textViewport = taRect;
        inputField.textComponent = txtTMP;
        inputField.placeholder = phTMP;
        inputField.characterLimit = 6;
        inputField.contentType = TMP_InputField.ContentType.Alphanumeric;

        return go;
    }
}
