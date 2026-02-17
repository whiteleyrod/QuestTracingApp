using UnityEngine;
using UnityEngine.SceneManagement;
using TracingApp.Data;

namespace TracingApp.Core
{
    /// <summary>
    /// Singleton GameManager that persists across scenes.
    /// Holds the current user ID, session settings, and session data.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Current Session Info")]
        public string UserID = "Anon";
        public SessionSettings CurrentSettings = new SessionSettings();
        public SessionData CurrentSession;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Set the user ID. If empty or null, defaults to "Anon".
        /// </summary>
        public void SetUserID(string id)
        {
            UserID = string.IsNullOrWhiteSpace(id) ? "Anon" : id.Trim();
            Debug.Log($"[GameManager] User ID set to: {UserID}");
        }

        /// <summary>
        /// Start a new session with the current settings.
        /// </summary>
        public void StartNewSession()
        {
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            CurrentSession = new SessionData
            {
                UserID = UserID,
                SessionID = timestamp,
                SessionDate = System.DateTime.Now.ToString("yyyy-MM-dd"),
                Settings = CurrentSettings.Clone()
            };

            Debug.Log($"[GameManager] New session started: {CurrentSession.SessionID} for user {UserID}");
        }

        /// <summary>
        /// Save the current session data to CSV files.
        /// </summary>
        public void SaveSessionData()
        {
            if (CurrentSession == null)
            {
                Debug.LogWarning("[GameManager] No session to save.");
                return;
            }

            CurrentSession.CalculateSummary();
            CSVWriter.WriteTrialData(CurrentSession);
            CSVWriter.WriteSessionSummary(CurrentSession);
            Debug.Log("[GameManager] Session data saved.");
        }

        // Scene navigation helpers
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadSettings()
        {
            SceneManager.LoadScene("Settings");
        }

        public void LoadTracingSession()
        {
            StartNewSession();
            SceneManager.LoadScene("TracingSession");
        }

        public void LoadResults()
        {
            SceneManager.LoadScene("Results");
        }

        public void QuitApp()
        {
            Debug.Log("[GameManager] Quitting application.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
