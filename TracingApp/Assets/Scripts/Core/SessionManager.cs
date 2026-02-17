using System;
using System.Collections.Generic;
using UnityEngine;
using TracingApp.Data;
using TracingApp.Patterns;

namespace TracingApp.Core
{
    /// <summary>
    /// Manages the flow of a tracing session: pattern sequence, timing, and transitions.
    /// Lives in the TracingSession scene.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }

        [Header("Session State")]
        public bool IsSessionActive = false;
        public int CurrentTrialIndex = 0;
        public float SessionTimeRemaining;
        public float SessionElapsedTime;

        [Header("References")]
        public TrialManager TrialManager;

        // Events
        public event Action OnSessionStarted;
        public event Action OnSessionEnded;
        public event Action<int, int> OnTrialChanged; // currentTrial, totalTrials
        public event Action<float> OnTimeUpdated;      // timeRemaining

        private List<PatternInfo> _sessionPatterns;
        private SessionSettings _settings;
        private bool _hasTimeLimit;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[SessionManager] GameManager not found! Make sure it exists in the scene or a previous scene.");
                return;
            }

            _settings = GameManager.Instance.CurrentSession?.Settings ?? new SessionSettings();
            InitializeSession();
        }

        private void Update()
        {
            if (!IsSessionActive) return;

            SessionElapsedTime += Time.deltaTime;

            if (_hasTimeLimit)
            {
                SessionTimeRemaining -= Time.deltaTime;
                OnTimeUpdated?.Invoke(SessionTimeRemaining);

                if (SessionTimeRemaining <= 0f)
                {
                    Debug.Log("[SessionManager] Time limit reached. Ending session.");
                    EndSession();
                }
            }
        }

        /// <summary>
        /// Initialize the session: select patterns and prepare.
        /// </summary>
        public void InitializeSession()
        {
            _sessionPatterns = PatternSelector.SelectPatterns(_settings);
            CurrentTrialIndex = 0;
            SessionElapsedTime = 0f;
            _hasTimeLimit = _settings.TimeLimitMinutes > 0;
            SessionTimeRemaining = _settings.TimeLimitMinutes * 60f;

            Debug.Log($"[SessionManager] Session initialized with {_sessionPatterns.Count} patterns.");

            IsSessionActive = true;
            OnSessionStarted?.Invoke();

            StartNextTrial();
        }

        /// <summary>
        /// Start the next trial in the sequence.
        /// </summary>
        public void StartNextTrial()
        {
            if (CurrentTrialIndex >= _sessionPatterns.Count)
            {
                EndSession();
                return;
            }

            PatternInfo pattern = _sessionPatterns[CurrentTrialIndex];
            OnTrialChanged?.Invoke(CurrentTrialIndex + 1, _sessionPatterns.Count);

            if (TrialManager != null)
            {
                TrialManager.StartTrial(pattern, _settings, CurrentTrialIndex + 1);
            }
            else
            {
                Debug.LogError("[SessionManager] TrialManager reference is missing!");
            }
        }

        /// <summary>
        /// Called by TrialManager when a trial is completed.
        /// </summary>
        public void OnTrialCompleted(TrialData trialData)
        {
            if (GameManager.Instance?.CurrentSession != null)
            {
                GameManager.Instance.CurrentSession.Trials.Add(trialData);
            }

            CurrentTrialIndex++;

            // Check if session should end
            if (CurrentTrialIndex >= _sessionPatterns.Count)
            {
                EndSession();
            }
            // Otherwise, TrialManager will show results, then call AdvanceToNextTrial
        }

        /// <summary>
        /// Called after results display to advance to the next trial.
        /// </summary>
        public void AdvanceToNextTrial()
        {
            if (CurrentTrialIndex < _sessionPatterns.Count && IsSessionActive)
            {
                StartNextTrial();
            }
            else
            {
                EndSession();
            }
        }

        /// <summary>
        /// End the session and save data.
        /// </summary>
        public void EndSession()
        {
            if (!IsSessionActive) return;

            IsSessionActive = false;
            Debug.Log("[SessionManager] Session ended.");

            // Save data
            GameManager.Instance?.SaveSessionData();

            OnSessionEnded?.Invoke();

            // Navigate to results
            GameManager.Instance?.LoadResults();
        }

        /// <summary>
        /// Get the current pattern info.
        /// </summary>
        public PatternInfo GetCurrentPattern()
        {
            if (_sessionPatterns != null && CurrentTrialIndex < _sessionPatterns.Count)
                return _sessionPatterns[CurrentTrialIndex];
            return null;
        }

        /// <summary>
        /// Get total pattern count for this session.
        /// </summary>
        public int GetTotalPatterns()
        {
            return _sessionPatterns?.Count ?? 0;
        }
    }
}
