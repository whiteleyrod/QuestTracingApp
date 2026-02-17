using System.Collections.Generic;
using System.Linq;
using TracingApp.Data;

namespace TracingApp.Scoring
{
    /// <summary>
    /// Aggregates trial scores into session-level statistics.
    /// </summary>
    public static class SessionScorer
    {
        public struct SessionStats
        {
            public float AvgAccuracy;
            public float AvgCoverage;
            public float AvgTime;
            public float MinAccuracy;
            public float MaxAccuracy;
            public float MinCoverage;
            public float MaxCoverage;
            public float MinTime;
            public float MaxTime;
            public float TotalTime;
            public int TotalTrials;
        }

        /// <summary>
        /// Calculate aggregate statistics for a list of trials.
        /// </summary>
        public static SessionStats CalculateStats(List<TrialData> trials)
        {
            var stats = new SessionStats();

            if (trials == null || trials.Count == 0)
                return stats;

            stats.TotalTrials = trials.Count;
            stats.MinAccuracy = trials.Min(t => t.AccuracyPercent);
            stats.MaxAccuracy = trials.Max(t => t.AccuracyPercent);
            stats.MinCoverage = trials.Min(t => t.CoveragePercent);
            stats.MaxCoverage = trials.Max(t => t.CoveragePercent);
            stats.MinTime = trials.Min(t => t.DurationSeconds);
            stats.MaxTime = trials.Max(t => t.DurationSeconds);
            stats.TotalTime = trials.Sum(t => t.DurationSeconds);
            stats.AvgAccuracy = trials.Average(t => t.AccuracyPercent);
            stats.AvgCoverage = trials.Average(t => t.CoveragePercent);
            stats.AvgTime = trials.Average(t => t.DurationSeconds);

            return stats;
        }
    }
}
