using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TracingApp.Data;
using TracingApp.Core;

namespace TracingApp.UI
{
    /// <summary>
    /// Displays trial results (shown for 10 seconds after each trial).
    /// Shows time, accuracy, and coverage.
    /// </summary>
    public class ResultsDisplay : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI TimeText;
        public TextMeshProUGUI AccuracyText;
        public TextMeshProUGUI CoverageText;
        public TextMeshProUGUI PatternNameText;
        public TextMeshProUGUI TrialCounterText;
        public Button NextButton;
        public GameObject ResultsPanel;

        [Header("Timer")]
        public TextMeshProUGUI CountdownText;

        private void Start()
        {
            if (NextButton != null)
                NextButton.onClick.AddListener(OnNextPressed);

            Hide();
        }

        /// <summary>
        /// Show the results for a completed trial.
        /// </summary>
        public void ShowResults(TrialData trialData)
        {
            if (ResultsPanel != null)
                ResultsPanel.SetActive(true);

            if (TimeText != null)
                TimeText.text = $"Time: {trialData.DurationSeconds:F1} seconds";

            if (AccuracyText != null)
                AccuracyText.text = $"Accuracy: {trialData.AccuracyPercent:F1}% Correct";

            if (CoverageText != null)
                CoverageText.text = $"Coverage: {trialData.CoveragePercent:F1}% Completed";

            if (PatternNameText != null)
                PatternNameText.text = trialData.PatternName;

            if (TrialCounterText != null && SessionManager.Instance != null)
            {
                int current = SessionManager.Instance.CurrentTrialIndex;
                int total = SessionManager.Instance.GetTotalPatterns();
                TrialCounterText.text = $"Pattern {current} of {total}";
            }
        }

        /// <summary>
        /// Update the countdown timer display.
        /// </summary>
        public void UpdateCountdown(float secondsRemaining)
        {
            if (CountdownText != null)
                CountdownText.text = $"Next in {Mathf.CeilToInt(secondsRemaining)}s";
        }

        /// <summary>
        /// Hide the results panel.
        /// </summary>
        public void Hide()
        {
            if (ResultsPanel != null)
                ResultsPanel.SetActive(false);
        }

        private void OnNextPressed()
        {
            // Find the TrialManager and skip results
            var trialManager = FindFirstObjectByType<TrialManager>();
            if (trialManager != null)
            {
                trialManager.SkipResults();
            }

            Hide();
        }
    }
}
