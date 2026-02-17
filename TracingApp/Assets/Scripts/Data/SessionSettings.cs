using System;

namespace TracingApp.Data
{
    /// <summary>
    /// Holds all configurable settings for a tracing session.
    /// </summary>
    [Serializable]
    public class SessionSettings
    {
        // Session configuration
        public int TimeLimitMinutes = 5;
        public int PatternCount = 5;
        public bool RandomizePatterns = false;

        // Pattern configuration
        public int Complexity = 1;       // 1-5
        public int LineThickness = 3;    // 1-5 (maps to 10-50px)
        public int Scale = 5;            // 1-10 index into scale sizes
        public bool RandomizeLevels = false;

        // Derived values
        public int LineThicknessPixels => LineThickness * 10; // 10, 20, 30, 40, 50

        public static readonly string[] ScaleLabels = new string[]
        {
            "512x512", "960x540", "1280x720", "1920x1080", "2560x1440",
            "3840x2160", "4096x2304", "5120x2880", "5760x3240", "7680x4320"
        };

        public static readonly int[][] ScaleSizes = new int[][]
        {
            new[] { 512, 512 },
            new[] { 960, 540 },
            new[] { 1280, 720 },
            new[] { 1920, 1080 },
            new[] { 2560, 1440 },
            new[] { 3840, 2160 },
            new[] { 4096, 2304 },
            new[] { 5120, 2880 },
            new[] { 5760, 3240 },
            new[] { 7680, 4320 }
        };

        public string GetScaleString()
        {
            int idx = UnityEngine.Mathf.Clamp(Scale - 1, 0, ScaleLabels.Length - 1);
            return ScaleLabels[idx];
        }

        public SessionSettings Clone()
        {
            return new SessionSettings
            {
                TimeLimitMinutes = this.TimeLimitMinutes,
                PatternCount = this.PatternCount,
                RandomizePatterns = this.RandomizePatterns,
                Complexity = this.Complexity,
                LineThickness = this.LineThickness,
                Scale = this.Scale,
                RandomizeLevels = this.RandomizeLevels
            };
        }
    }
}
