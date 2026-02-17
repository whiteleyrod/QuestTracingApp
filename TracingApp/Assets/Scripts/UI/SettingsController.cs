using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TracingApp.Core;
using TracingApp.Data;

namespace TracingApp.UI
{
    /// <summary>
    /// Controls the Settings screen UI.
    /// Manages time limit, pattern count, complexity, thickness, and scale settings.
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        [Header("Session Settings UI")]
        public TextMeshProUGUI TimeLimitText;
        public Button TimeLimitUpButton;
        public Button TimeLimitDownButton;

        public TextMeshProUGUI PatternCountText;
        public Button PatternCountUpButton;
        public Button PatternCountDownButton;

        public Button RandomizePatternsButton;

        [Header("Level Settings UI")]
        public Slider ComplexitySlider;
        public TextMeshProUGUI ComplexityLabel;

        public Slider ThicknessSlider;
        public TextMeshProUGUI ThicknessLabel;

        public Slider ScaleSlider;
        public TextMeshProUGUI ScaleLabel;

        [Header("Difficulty Presets UI")]
        public Button[] PresetButtons;

        public Button RandomizeLevelsButton;

        [Header("Navigation")]
        public Button StartButton;
        public Button BackButton;

        [Header("Settings")]
        public int MinTimeLimit = 1;
        public int MaxTimeLimit = 30;
        public int MinPatternCount = 1;
        public int MaxPatternCount = 20;

        private SessionSettings _settings;
        private int _selectedPresetIndex = -1;

        private struct DifficultyPreset
        {
            public string Name;
            public int Complexity;
            public int LineThickness;
            public int Scale;

            public DifficultyPreset(string name, int complexity, int lineThickness, int scale)
            {
                Name = name;
                Complexity = complexity;
                LineThickness = lineThickness;
                Scale = scale;
            }
        }

        private static readonly DifficultyPreset[] DifficultyPresets =
        {
            new DifficultyPreset("I'm new here", 1, 5, 1),
            new DifficultyPreset("Getting the hang", 2, 4, 3),
            new DifficultyPreset("Steady hands", 3, 3, 5),
            new DifficultyPreset("Bring it on", 4, 2, 8),
            new DifficultyPreset("Woah!", 5, 1, 10)
        };

        private static readonly string[] ComplexityNames = { "Simple", "Easy", "Medium", "Hard", "Complex" };
        private static readonly string[] ThicknessNames = { "Thin (10px)", "Light (20px)", "Medium (30px)", "Thick (40px)", "Very Thick (50px)" };
        private static readonly Color PresetButtonNormalColor = new Color(0.3f, 0.3f, 0.4f, 1f);
        private static readonly Color PresetButtonSelectedColor = new Color(0.2f, 0.6f, 0.2f, 1f);

        private void Start()
        {
            _settings = GameManager.Instance?.CurrentSettings ?? new SessionSettings();

            EnsurePresetUI();

            SetupButtons();
            SetupSliders();

            // Default preset when opening settings
            ApplyPreset(0);
        }

        private void SetupButtons()
        {
            if (TimeLimitUpButton != null)
                TimeLimitUpButton.onClick.AddListener(() => AdjustTimeLimit(1));
            if (TimeLimitDownButton != null)
                TimeLimitDownButton.onClick.AddListener(() => AdjustTimeLimit(-1));

            if (PatternCountUpButton != null)
                PatternCountUpButton.onClick.AddListener(() => AdjustPatternCount(1));
            if (PatternCountDownButton != null)
                PatternCountDownButton.onClick.AddListener(() => AdjustPatternCount(-1));

            if (RandomizePatternsButton != null)
                RandomizePatternsButton.onClick.AddListener(OnRandomizePatterns);

            if (RandomizeLevelsButton != null)
                RandomizeLevelsButton.onClick.AddListener(OnRandomizeLevels);

            if (PresetButtons != null)
            {
                for (int i = 0; i < Mathf.Min(PresetButtons.Length, DifficultyPresets.Length); i++)
                {
                    int presetIndex = i;
                    if (PresetButtons[presetIndex] != null)
                        PresetButtons[presetIndex].onClick.AddListener(() => ApplyPreset(presetIndex));
                }
            }

            if (StartButton != null)
                StartButton.onClick.AddListener(OnStartSession);

            if (BackButton != null)
                BackButton.onClick.AddListener(OnBack);
        }

        private void SetupSliders()
        {
            if (ComplexitySlider != null)
            {
                ComplexitySlider.minValue = 1;
                ComplexitySlider.maxValue = 5;
                ComplexitySlider.wholeNumbers = true;
                ComplexitySlider.value = _settings.Complexity;
                ComplexitySlider.onValueChanged.AddListener(OnComplexityChanged);
            }

            if (ThicknessSlider != null)
            {
                ThicknessSlider.minValue = 1;
                ThicknessSlider.maxValue = 5;
                ThicknessSlider.wholeNumbers = true;
                ThicknessSlider.value = _settings.LineThickness;
                ThicknessSlider.onValueChanged.AddListener(OnThicknessChanged);
            }

            if (ScaleSlider != null)
            {
                ScaleSlider.minValue = 1;
                ScaleSlider.maxValue = 10;
                ScaleSlider.wholeNumbers = true;
                ScaleSlider.value = _settings.Scale;
                ScaleSlider.onValueChanged.AddListener(OnScaleChanged);
            }
        }

        private void AdjustTimeLimit(int delta)
        {
            _settings.TimeLimitMinutes = Mathf.Clamp(_settings.TimeLimitMinutes + delta, MinTimeLimit, MaxTimeLimit);
            UpdateTimeLimitUI();
        }

        private void AdjustPatternCount(int delta)
        {
            _settings.PatternCount = Mathf.Clamp(_settings.PatternCount + delta, MinPatternCount, MaxPatternCount);
            UpdatePatternCountUI();
        }

        private void OnComplexityChanged(float value)
        {
            _settings.Complexity = Mathf.RoundToInt(value);
            UpdateSelectedPresetFromCurrentSettings();
            UpdateComplexityUI();
            UpdatePresetUI();
        }

        private void OnThicknessChanged(float value)
        {
            _settings.LineThickness = Mathf.RoundToInt(value);
            UpdateSelectedPresetFromCurrentSettings();
            UpdateThicknessUI();
            UpdatePresetUI();
        }

        private void OnScaleChanged(float value)
        {
            _settings.Scale = Mathf.RoundToInt(value);
            UpdateSelectedPresetFromCurrentSettings();
            UpdateScaleUI();
            UpdatePresetUI();
        }

        private void OnRandomizePatterns()
        {
            _settings.TimeLimitMinutes = Random.Range(MinTimeLimit, MaxTimeLimit + 1);
            _settings.PatternCount = Random.Range(MinPatternCount, Mathf.Min(MaxPatternCount, 10) + 1);
            _settings.RandomizePatterns = true;
            UpdateAllUI();
        }

        private void OnRandomizeLevels()
        {
            _settings.Complexity = Random.Range(1, 6);
            _settings.LineThickness = Random.Range(1, 6);
            _settings.Scale = Random.Range(1, 11);
            _settings.RandomizeLevels = true;
            _selectedPresetIndex = FindMatchingPresetIndex();
            UpdateAllUI();

            // Update slider positions
            if (ComplexitySlider != null) ComplexitySlider.value = _settings.Complexity;
            if (ThicknessSlider != null) ThicknessSlider.value = _settings.LineThickness;
            if (ScaleSlider != null) ScaleSlider.value = _settings.Scale;
        }

        private void OnStartSession()
        {
            // Apply settings to GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentSettings = _settings;
                GameManager.Instance.LoadTracingSession();
            }
        }

        private void OnBack()
        {
            GameManager.Instance?.LoadMainMenu();
        }

        // UI Update Methods
        private void UpdateAllUI()
        {
            UpdateTimeLimitUI();
            UpdatePatternCountUI();
            UpdateComplexityUI();
            UpdateThicknessUI();
            UpdateScaleUI();
            UpdatePresetUI();
        }

        private void UpdateTimeLimitUI()
        {
            if (TimeLimitText != null)
                TimeLimitText.text = $"{_settings.TimeLimitMinutes} min";
        }

        private void UpdatePatternCountUI()
        {
            if (PatternCountText != null)
                PatternCountText.text = $"{_settings.PatternCount}";
        }

        private void UpdateComplexityUI()
        {
            if (ComplexityLabel != null)
            {
                int idx = Mathf.Clamp(_settings.Complexity - 1, 0, ComplexityNames.Length - 1);
                ComplexityLabel.text = $"{ComplexityNames[idx]} ({_settings.Complexity})";
            }
        }

        private void UpdateThicknessUI()
        {
            if (ThicknessLabel != null)
            {
                int idx = Mathf.Clamp(_settings.LineThickness - 1, 0, ThicknessNames.Length - 1);
                ThicknessLabel.text = ThicknessNames[idx];
            }
        }

        private void UpdateScaleUI()
        {
            if (ScaleLabel != null)
            {
                ScaleLabel.text = _settings.GetScaleString();
            }
        }

        private void ApplyPreset(int index)
        {
            _selectedPresetIndex = Mathf.Clamp(index, 0, DifficultyPresets.Length - 1);
            var preset = DifficultyPresets[Mathf.Clamp(index, 0, DifficultyPresets.Length - 1)];
            _settings.Complexity = preset.Complexity;
            _settings.LineThickness = preset.LineThickness;
            _settings.Scale = preset.Scale;
            _settings.RandomizeLevels = false;

            if (ComplexitySlider != null) ComplexitySlider.SetValueWithoutNotify(_settings.Complexity);
            if (ThicknessSlider != null) ThicknessSlider.SetValueWithoutNotify(_settings.LineThickness);
            if (ScaleSlider != null) ScaleSlider.SetValueWithoutNotify(_settings.Scale);

            UpdateAllUI();
        }

        private int FindMatchingPresetIndex()
        {
            for (int i = 0; i < DifficultyPresets.Length; i++)
            {
                var preset = DifficultyPresets[i];
                if (preset.Complexity == _settings.Complexity &&
                    preset.LineThickness == _settings.LineThickness &&
                    preset.Scale == _settings.Scale)
                {
                    return i;
                }
            }

            return -1;
        }

        private void UpdateSelectedPresetFromCurrentSettings()
        {
            _selectedPresetIndex = FindMatchingPresetIndex();
        }

        private void UpdatePresetUI()
        {
            if (PresetButtons == null)
                return;

            for (int i = 0; i < Mathf.Min(PresetButtons.Length, DifficultyPresets.Length); i++)
            {
                var button = PresetButtons[i];
                if (button == null)
                    continue;

                var image = button.GetComponent<Image>();
                if (image != null)
                    image.color = (i == _selectedPresetIndex) ? PresetButtonSelectedColor : PresetButtonNormalColor;
            }
        }

        private void EnsurePresetUI()
        {
            if (HasValidPresetButtons())
                return;

            Transform levelPanel = FindTransformByName("LevelSettingsPanel");
            if (levelPanel == null)
                return;

            Transform existingRow = levelPanel.Find("DifficultyPresetRow");
            GameObject row;
            if (existingRow != null)
            {
                row = existingRow.gameObject;
            }
            else
            {
                row = new GameObject("DifficultyPresetRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
                row.transform.SetParent(levelPanel, false);
                row.transform.SetSiblingIndex(1);
            }

            for (int i = row.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(row.transform.GetChild(i).gameObject);
            }

            var rowLayout = row.GetComponent<HorizontalLayoutGroup>();
            if (rowLayout == null)
                rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 10;
            rowLayout.childAlignment = TextAnchor.MiddleCenter;
            rowLayout.childControlWidth = false;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;

            var rowElement = row.GetComponent<LayoutElement>();
            if (rowElement == null)
                rowElement = row.AddComponent<LayoutElement>();
            rowElement.preferredHeight = 56;

            PresetButtons = new Button[DifficultyPresets.Length];
            for (int i = 0; i < DifficultyPresets.Length; i++)
            {
                PresetButtons[i] = CreateInlinePresetButton(row.transform, $"PresetButton{i + 1}", DifficultyPresets[i].Name);
            }
        }

        private Button CreateInlinePresetButton(Transform parent, string objectName, string caption)
        {
            var buttonGO = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGO.transform.SetParent(parent, false);

            var image = buttonGO.GetComponent<Image>();
            image.color = PresetButtonNormalColor;

            var layout = buttonGO.AddComponent<LayoutElement>();
            layout.preferredWidth = 145;
            layout.preferredHeight = 46;

            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(buttonGO.transform, false);
            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = caption;
            text.fontSize = 16;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return buttonGO.GetComponent<Button>();
        }

        private bool HasValidPresetButtons()
        {
            if (PresetButtons == null || PresetButtons.Length < DifficultyPresets.Length)
                return false;

            for (int i = 0; i < DifficultyPresets.Length; i++)
            {
                if (PresetButtons[i] == null)
                    return false;
            }

            return true;
        }

        private Transform FindTransformByName(string targetName)
        {
            var allTransforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);
            for (int i = 0; i < allTransforms.Length; i++)
            {
                if (allTransforms[i].name == targetName)
                    return allTransforms[i];
            }

            return null;
        }
    }
}
