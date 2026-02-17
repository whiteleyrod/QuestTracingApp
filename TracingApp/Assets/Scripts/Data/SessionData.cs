using System;
using System.Collections.Generic;

namespace TracingApp.Data
{
    /// <summary>
    /// Stores all data for a complete session including all trials.
    /// </summary>
    [Serializable]
    public class SessionData
    {
        public string UserID;
        public string SessionID;
        public string SessionDate;
        public int TotalTrials;
        public float TotalDurationSeconds;
        public float AvgAccuracy;
        public float AvgCoverage;
        public float AvgTime;
        public SessionSettings Settings;
        public List<TrialData> Trials = new List<TrialData>();

        public void CalculateSummary()
        {
            TotalTrials = Trials.Count;
            if (TotalTrials == 0) return;

            float totalAccuracy = 0f;
            float totalCoverage = 0f;
            float totalTime = 0f;

            foreach (var trial in Trials)
            {
                totalAccuracy += trial.AccuracyPercent;
                totalCoverage += trial.CoveragePercent;
                totalTime += trial.DurationSeconds;
            }

            TotalDurationSeconds = totalTime;
            AvgAccuracy = totalAccuracy / TotalTrials;
            AvgCoverage = totalCoverage / TotalTrials;
            AvgTime = totalTime / TotalTrials;
        }

        public string ToSummaryCSVHeader()
        {
            return "UserID,SessionID,SessionDate,TotalTrials,TotalDuration_Seconds," +
                   "AvgAccuracy,AvgCoverage,AvgTime,SettingsComplexity,SettingsThickness," +
                   "SettingsScale,SettingsTimeLimit,SettingsPatternCount";
        }

        public string ToSummaryCSVRow()
        {
            return $"{UserID},{SessionID},{SessionDate},{TotalTrials},{TotalDurationSeconds:F1}," +
                   $"{AvgAccuracy:F1},{AvgCoverage:F1},{AvgTime:F1},{Settings?.Complexity}," +
                   $"{Settings?.LineThicknessPixels},{Settings?.GetScaleString()}," +
                   $"{Settings?.TimeLimitMinutes},{Settings?.PatternCount}";
        }
    }
}
