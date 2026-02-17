using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TracingApp.Core;
using TracingApp.Data;
using TracingApp.Scoring;

namespace TracingApp.UI
{
    /// <summary>
    /// Displays the session summary screen with all trial results.
    /// </summary>
    public class SessionSummary : MonoBehaviour
    {
        [Header("Summary UI")]
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI OverallScoreText;
        public TextMeshProUGUI AvgAccuracyText;
        public TextMeshProUGUI AvgCoverageText;
        public TextMeshProUGUI AvgTimeText;
        public TextMeshProUGUI TotalTimeText;
        public TextMeshProUGUI TotalTrialsText;

        [Header("Trial List")]
        public Transform TrialListContainer;
        public GameObject TrialRowPrefab;

        [Header("Navigation")]
        public Button NewSessionButton;
        public Button MainMenuButton;
        public Button ExitButton;

        private void Start()
        {
            if (NewSessionButton != null)
                NewSessionButton.onClick.AddListener(OnNewSession);
            if (MainMenuButton != null)
                MainMenuButton.onClick.AddListener(OnMainMenu);
            if (ExitButton != null)
                ExitButton.onClick.AddListener(OnExit);

            PopulateSummary();
        }

        /// <summary>
        /// Populate the summary screen with session data.
        /// </summary>
        private void PopulateSummary()
        {
            var session = GameManager.Instance?.CurrentSession;
            if (session == null)
            {
                Debug.LogWarning("[SessionSummary] No session data available.");
                return;
            }

            session.CalculateSummary();
            var stats = SessionScorer.CalculateStats(session.Trials);

            if (TitleText != null)
                TitleText.text = $"Session Summary - {session.UserID}";

            if (OverallScoreText != null)
            {
                float overallScore = (stats.AvgAccuracy + stats.AvgCoverage) / 2f;
                OverallScoreText.text = $"Overall Score: {overallScore:F1}%";
            }

            if (AvgAccuracyText != null)
                AvgAccuracyText.text = $"Avg Accuracy: {stats.AvgAccuracy:F1}%";

            if (AvgCoverageText != null)
                AvgCoverageText.text = $"Avg Coverage: {stats.AvgCoverage:F1}%";

            if (AvgTimeText != null)
                AvgTimeText.text = $"Avg Time: {stats.AvgTime:F1}s";

            if (TotalTimeText != null)
                TotalTimeText.text = $"Total Time: {stats.TotalTime:F1}s";

            if (TotalTrialsText != null)
                TotalTrialsText.text = $"Trials Completed: {stats.TotalTrials}";

            // Populate trial rows
            PopulateTrialList(session);
        }

        /// <summary>
        /// Create UI rows for each trial in the session.
        /// </summary>
        private void PopulateTrialList(SessionData session)
        {
            if (TrialListContainer == null || TrialRowPrefab == null) return;

            // Clear existing rows
            foreach (Transform child in TrialListContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var trial in session.Trials)
            {
                GameObject row = Instantiate(TrialRowPrefab, TrialListContainer);
                var texts = row.GetComponentsInChildren<TextMeshProUGUI>();

                // Expected layout: PatternName | Time | Accuracy | Coverage
                if (texts.Length >= 4)
                {
                    texts[0].text = trial.PatternName;
                    texts[1].text = $"{trial.DurationSeconds:F1}s";
                    texts[2].text = $"{trial.AccuracyPercent:F1}%";
                    texts[3].text = $"{trial.CoveragePercent:F1}%";
                }
                else if (texts.Length >= 1)
                {
                    texts[0].text = $"{trial.PatternName} - Time: {trial.DurationSeconds:F1}s | Accuracy: {trial.AccuracyPercent:F1}% | Coverage: {trial.CoveragePercent:F1}%";
                }
            }
        }

        private void OnNewSession()
        {
            GameManager.Instance?.LoadSettings();
        }

        private void OnMainMenu()
        {
            GameManager.Instance?.LoadMainMenu();
        }

        private void OnExit()
        {
            GameManager.Instance?.QuitApp();
        }
    }
}
