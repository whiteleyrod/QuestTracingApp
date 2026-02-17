using System;

namespace TracingApp.Data
{
    /// <summary>
    /// Stores all data for a single tracing trial.
    /// </summary>
    [Serializable]
    public class TrialData
    {
        public string UserID;
        public string SessionID;
        public int TrialNumber;
        public string PatternName;
        public int Complexity;
        public int LineThickness;
        public string Scale;
        public string TimeStarted;
        public string TimeCompleted;
        public float DurationSeconds;
        public int PixelsTraced;
        public int PixelsInTarget;
        public int PixelsOutTarget;
        public float AccuracyPercent;
        public int TargetPixelsTotal;
        public int TargetPixelsCovered;
        public float CoveragePercent;
        public string ScreenshotFilename;

        public void Calculate()
        {
            PixelsOutTarget = PixelsTraced - PixelsInTarget;
            if (PixelsTraced > 0)
                AccuracyPercent = (float)PixelsInTarget / PixelsTraced * 100f;
            else
                AccuracyPercent = 0f;

            if (TargetPixelsTotal > 0)
                CoveragePercent = (float)TargetPixelsCovered / TargetPixelsTotal * 100f;
            else
                CoveragePercent = 0f;
        }

        public string ToCSVHeader()
        {
            return "UserID,SessionID,TrialNumber,PatternName,Complexity,LineThickness,Scale," +
                   "TimeStarted,TimeCompleted,Duration_Seconds,PixelsTraced,PixelsInTarget," +
                   "PixelsOutTarget,AccuracyPercent,TargetPixelsTotal,TargetPixelsCovered," +
                   "CoveragePercent,ScreenshotFilename";
        }

        public string ToCSVRow()
        {
            return $"{UserID},{SessionID},{TrialNumber},{PatternName},{Complexity},{LineThickness}," +
                   $"{Scale},{TimeStarted},{TimeCompleted},{DurationSeconds:F1},{PixelsTraced}," +
                   $"{PixelsInTarget},{PixelsOutTarget},{AccuracyPercent:F1},{TargetPixelsTotal}," +
                   $"{TargetPixelsCovered},{CoveragePercent:F1},{ScreenshotFilename}";
        }
    }
}
