using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TracingApp.Data;

namespace TracingApp.Patterns
{
    /// <summary>
    /// Selects patterns for a session based on settings.
    /// </summary>
    public static class PatternSelector
    {
        /// <summary>
        /// Select patterns for a session based on the given settings.
        /// </summary>
        public static List<PatternInfo> SelectPatterns(SessionSettings settings)
        {
            List<PatternInfo> available;

            if (settings.RandomizeLevels)
            {
                // Use all patterns when randomized
                available = new List<PatternInfo>(PatternInfo.AllPatterns);
            }
            else
            {
                // Filter by complexity level
                available = PatternInfo.AllPatterns
                    .Where(p => p.Complexity == settings.Complexity)
                    .ToList();
            }

            if (available.Count == 0)
            {
                Debug.LogWarning("[PatternSelector] No patterns available for the given settings. Using all patterns.");
                available = new List<PatternInfo>(PatternInfo.AllPatterns);
            }

            List<PatternInfo> selected = new List<PatternInfo>();
            int count = Mathf.Min(settings.PatternCount, available.Count);

            if (settings.RandomizePatterns)
            {
                // Shuffle and pick
                ShuffleList(available);
                for (int i = 0; i < count; i++)
                {
                    selected.Add(available[i]);
                }

                // If we need more patterns than available, repeat
                while (selected.Count < settings.PatternCount)
                {
                    ShuffleList(available);
                    for (int i = 0; i < available.Count && selected.Count < settings.PatternCount; i++)
                    {
                        selected.Add(available[i]);
                    }
                }
            }
            else
            {
                // Sequential selection
                for (int i = 0; i < settings.PatternCount; i++)
                {
                    selected.Add(available[i % available.Count]);
                }
            }

            return selected;
        }

        /// <summary>
        /// Get all patterns for a specific complexity level.
        /// </summary>
        public static List<PatternInfo> GetPatternsByComplexity(int complexity)
        {
            return PatternInfo.AllPatterns
                .Where(p => p.Complexity == complexity)
                .ToList();
        }

        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
