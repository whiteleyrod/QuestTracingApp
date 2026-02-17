using UnityEngine;
using TMPro;
using TracingApp.Core;

namespace TracingApp.UI
{
    /// <summary>
    /// In-session HUD showing trial counter, time remaining, and recording state.
    /// </summary>
    public class SessionHUD : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI TrialCounterText;
        public TextMeshProUGUI TimeRemainingText;
        public TextMeshProUGUI TrialTimerText;
        public RecordingIndicator RecordingIndicator;
        public ResultsDisplay ResultsDisplay;

        private SessionManager _sessionManager;
        private TrialManager _trialManager;

        private void Start()
        {
            _sessionManager = SessionManager.Instance;
            _trialManager = FindFirstObjectByType<TrialManager>();

            if (_sessionManager != null)
            {
                _sessionManager.OnTrialChanged += UpdateTrialCounter;
                _sessionManager.OnTimeUpdated += UpdateTimeRemaining;
            }

            if (_trialManager != null)
            {
                _trialManager.OnRecordingStarted += OnRecordingStarted;
                _trialManager.OnRecordingStopped += OnRecordingStopped;
                _trialManager.OnTrialResultReady += OnTrialResultReady;
            }
        }

        private void Update()
        {
            // Update trial timer
            if (_trialManager != null && _trialManager.IsRecording)
            {
                if (TrialTimerText != null)
                    TrialTimerText.text = $"{_trialManager.TrialTimer:F1}s";
            }
        }

        private void UpdateTrialCounter(int current, int total)
        {
            if (TrialCounterText != null)
                TrialCounterText.text = $"Pattern {current} of {total}";
        }

        private void UpdateTimeRemaining(float timeRemaining)
        {
            if (TimeRemainingText != null)
            {
                if (timeRemaining > 0)
                {
                    int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                    int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                    TimeRemainingText.text = $"Time: {minutes:00}:{seconds:00}";
                }
                else
                {
                    TimeRemainingText.text = "Time: 00:00";
                }
            }
        }

        private void OnRecordingStarted()
        {
            if (RecordingIndicator != null)
                RecordingIndicator.SetActive(true);

            if (TrialTimerText != null)
                TrialTimerText.gameObject.SetActive(true);
        }

        private void OnRecordingStopped()
        {
            if (RecordingIndicator != null)
                RecordingIndicator.SetActive(false);
        }

        private void OnTrialResultReady(Data.TrialData trialData)
        {
            if (ResultsDisplay != null)
                ResultsDisplay.ShowResults(trialData);
        }

        private void OnDestroy()
        {
            if (_sessionManager != null)
            {
                _sessionManager.OnTrialChanged -= UpdateTrialCounter;
                _sessionManager.OnTimeUpdated -= UpdateTimeRemaining;
            }

            if (_trialManager != null)
            {
                _trialManager.OnRecordingStarted -= OnRecordingStarted;
                _trialManager.OnRecordingStopped -= OnRecordingStopped;
                _trialManager.OnTrialResultReady -= OnTrialResultReady;
            }
        }
    }
}
