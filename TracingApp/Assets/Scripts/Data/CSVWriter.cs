using System.IO;
using System.Text;
using UnityEngine;

namespace TracingApp.Data
{
    /// <summary>
    /// Handles writing session and trial data to CSV files.
    /// </summary>
    public static class CSVWriter
    {
        /// <summary>
        /// Get the base data directory path.
        /// On Quest, uses Application.persistentDataPath.
        /// </summary>
        public static string GetDataDirectory()
        {
            return Path.Combine(Application.persistentDataPath, "Data", "Sessions");
        }

        /// <summary>
        /// Get the screenshots directory for a specific user and session.
        /// </summary>
        public static string GetScreenshotDirectory(string userID, string sessionTimestamp)
        {
            return Path.Combine(Application.persistentDataPath, "Data", "Screenshots", userID, sessionTimestamp);
        }

        /// <summary>
        /// Write trial data CSV for a session.
        /// </summary>
        public static string WriteTrialData(SessionData session)
        {
            string dir = GetDataDirectory();
            Directory.CreateDirectory(dir);

            string filename = $"{session.UserID}_{session.SessionID}_trials.csv";
            string filepath = Path.Combine(dir, filename);

            StringBuilder sb = new StringBuilder();

            // Header
            if (session.Trials.Count > 0)
                sb.AppendLine(session.Trials[0].ToCSVHeader());

            // Rows
            foreach (var trial in session.Trials)
            {
                sb.AppendLine(trial.ToCSVRow());
            }

            File.WriteAllText(filepath, sb.ToString());
            Debug.Log($"[CSVWriter] Trial data saved to: {filepath}");
            return filepath;
        }

        /// <summary>
        /// Write session summary CSV.
        /// </summary>
        public static string WriteSessionSummary(SessionData session)
        {
            string dir = GetDataDirectory();
            Directory.CreateDirectory(dir);

            string filename = $"{session.UserID}_{session.SessionID}_summary.csv";
            string filepath = Path.Combine(dir, filename);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(session.ToSummaryCSVHeader());
            sb.AppendLine(session.ToSummaryCSVRow());

            File.WriteAllText(filepath, sb.ToString());
            Debug.Log($"[CSVWriter] Session summary saved to: {filepath}");
            return filepath;
        }
    }
}
