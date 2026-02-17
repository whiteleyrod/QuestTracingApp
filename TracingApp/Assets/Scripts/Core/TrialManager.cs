using System;
using UnityEngine;
using UnityEngine.UI;
using TracingApp.Data;
using TracingApp.Patterns;
using TracingApp.Scoring;
using TracingApp.Tracing;

namespace TracingApp.Core
{
    /// <summary>
    /// Manages a single trial: pattern display, recording, scoring, and results.
    /// </summary>
    public class TrialManager : MonoBehaviour
    {
        [Header("Pattern Display")]
        [Tooltip("The RawImage on the world-space canvas that displays the pattern")]
        public RawImage PatternDisplay;

        [Header("Tracing")]
        public TracingRecorder TracingRecorder;

        [Header("Results Display")]
        public float ResultsDisplayDuration = 10f;

        // Events
        public event Action<TrialData> OnTrialResultReady;
        public event Action OnTrialStarted;
        public event Action OnRecordingStarted;
        public event Action OnRecordingStopped;

        // State
        public bool IsTrialActive { get; private set; }
        public bool IsRecording { get; private set; }
        public float TrialTimer { get; private set; }

        private PatternInfo _currentPattern;
        private SessionSettings _currentSettings;
        private int _trialNumber;
        private Texture2D _patternTexture;
        private TrialData _currentTrialData;
        private string _trialStartTime;
        private float _resultsTimer;
        private bool _showingResults;

        /// <summary>
        /// Start a new trial with the given pattern and settings.
        /// </summary>
        public void StartTrial(PatternInfo pattern, SessionSettings settings, int trialNumber)
        {
            _currentPattern = pattern;
            _currentSettings = settings;
            _trialNumber = trialNumber;
            IsTrialActive = true;
            IsRecording = false;
            TrialTimer = 0f;
            _showingResults = false;

            // Load pattern texture
            _patternTexture = PatternLoader.LoadPatternTexture(pattern, settings);

            if (_patternTexture == null)
            {
                Debug.LogError($"[TrialManager] Failed to load pattern: {pattern.Key}");
                return;
            }

            // Display pattern
            if (PatternDisplay != null)
            {
                PatternDisplay.texture = _patternTexture;
                PatternDisplay.gameObject.SetActive(true);
            }

            // Initialize tracing recorder
            if (TracingRecorder != null)
            {
                TracingRecorder.Initialize(_patternTexture.width, _patternTexture.height);
            }

            Debug.Log($"[TrialManager] Trial {trialNumber} started: {pattern.DisplayName} (Level {pattern.Complexity})");
            OnTrialStarted?.Invoke();
        }

        private void Update()
        {
            if (_showingResults)
            {
                _resultsTimer -= Time.deltaTime;
                if (_resultsTimer <= 0f)
                {
                    FinishResultsDisplay();
                }
                return;
            }

            if (!IsTrialActive) return;

            if (IsRecording)
            {
                TrialTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// Toggle recording on/off. Called when user presses trigger.
        /// </summary>
        public void ToggleRecording()
        {
            if (!IsTrialActive || _showingResults) return;

            if (!IsRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        /// <summary>
        /// Start recording the user's trace.
        /// </summary>
        public void StartRecording()
        {
            if (IsRecording) return;

            IsRecording = true;
            _trialStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            TrialTimer = 0f;

            if (TracingRecorder != null)
                TracingRecorder.StartRecording();

            Debug.Log("[TrialManager] Recording started.");
            OnRecordingStarted?.Invoke();
        }

        /// <summary>
        /// Stop recording and calculate scores.
        /// </summary>
        public void StopRecording()
        {
            if (!IsRecording) return;

            IsRecording = false;
            string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (TracingRecorder != null)
                TracingRecorder.StopRecording();

            Debug.Log("[TrialManager] Recording stopped.");
            OnRecordingStopped?.Invoke();

            // Create trial data
            _currentTrialData = new TrialData
            {
                UserID = GameManager.Instance?.UserID ?? "Anon",
                SessionID = GameManager.Instance?.CurrentSession?.SessionID ?? "unknown",
                TrialNumber = _trialNumber,
                PatternName = _currentPattern.Key,
                Complexity = _currentPattern.Complexity,
                LineThickness = _currentSettings.LineThicknessPixels,
                Scale = _currentSettings.GetScaleString(),
                TimeStarted = _trialStartTime,
                TimeCompleted = endTime,
                DurationSeconds = TrialTimer
            };

            // Score the trial
            if (TracingRecorder != null && _patternTexture != null)
            {
                Texture2D traceTexture = TracingRecorder.GetTraceTexture();
                TrialScorer.ScoreTrial(_patternTexture, traceTexture, _currentTrialData);
            }

            // Capture screenshot
            if (TracingRecorder != null)
            {
                string screenshotFile = ScreenshotCapture.CaptureTexture(
                    TracingRecorder.GetCompositeTexture(_patternTexture),
                    _currentTrialData.UserID,
                    _currentTrialData.SessionID,
                    _trialNumber
                );
                _currentTrialData.ScreenshotFilename = screenshotFile;
            }

            // Show results
            ShowResults();
        }

        /// <summary>
        /// Display trial results for the configured duration.
        /// </summary>
        private void ShowResults()
        {
            _showingResults = true;
            _resultsTimer = ResultsDisplayDuration;
            IsTrialActive = false;

            OnTrialResultReady?.Invoke(_currentTrialData);
            Debug.Log($"[TrialManager] Results - Accuracy: {_currentTrialData.AccuracyPercent:F1}%, Coverage: {_currentTrialData.CoveragePercent:F1}%, Time: {_currentTrialData.DurationSeconds:F1}s");
        }

        /// <summary>
        /// Skip the results display timer (called by "Next" button).
        /// </summary>
        public void SkipResults()
        {
            if (_showingResults)
            {
                FinishResultsDisplay();
            }
        }

        /// <summary>
        /// Finish showing results and notify SessionManager.
        /// </summary>
        private void FinishResultsDisplay()
        {
            _showingResults = false;

            // Notify session manager
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnTrialCompleted(_currentTrialData);
                SessionManager.Instance.AdvanceToNextTrial();
            }
        }
    }
}
