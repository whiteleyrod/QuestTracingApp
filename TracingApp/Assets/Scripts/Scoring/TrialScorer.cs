using UnityEngine;
using TracingApp.Data;

namespace TracingApp.Scoring
{
    /// <summary>
    /// Calculates accuracy and coverage scores for a single trial.
    /// Works by comparing the burn trail texture against the pattern mask texture.
    /// </summary>
    public static class TrialScorer
    {
        /// <summary>
        /// Score a trial by comparing the user's trace against the target pattern.
        /// </summary>
        /// <param name="patternTexture">The target pattern texture (white on transparent)</param>
        /// <param name="traceTexture">The user's burn trail texture</param>
        /// <param name="trialData">Trial data to populate with scores</param>
        /// <param name="alphaThreshold">Minimum alpha value to consider a pixel "drawn"</param>
        public static void ScoreTrial(Texture2D patternTexture, Texture2D traceTexture,
            TrialData trialData, byte alphaThreshold = 128)
        {
            if (patternTexture == null || traceTexture == null)
            {
                Debug.LogWarning("[TrialScorer] Cannot score: null texture(s).");
                return;
            }

            // Ensure textures are the same size for comparison
            int width = patternTexture.width;
            int height = patternTexture.height;

            Color32[] patternPixels = patternTexture.GetPixels32();
            Color32[] tracePixels;

            // If trace texture is different size, we need to resize
            if (traceTexture.width != width || traceTexture.height != height)
            {
                Debug.LogWarning($"[TrialScorer] Texture size mismatch. Pattern: {width}x{height}, Trace: {traceTexture.width}x{traceTexture.height}. Resizing trace.");
                tracePixels = ResamplePixels(traceTexture, width, height);
            }
            else
            {
                tracePixels = traceTexture.GetPixels32();
            }

            int totalPixels = width * height;
            int targetPixelsTotal = 0;
            int pixelsTraced = 0;
            int pixelsInTarget = 0;
            int targetPixelsCovered = 0;

            bool[] targetMask = new bool[totalPixels];
            bool[] traceMask = new bool[totalPixels];
            int tracePixelCount = 0;

            for (int i = 0; i < totalPixels; i++)
            {
                bool isTarget = patternPixels[i].a >= alphaThreshold;
                bool isTraced = tracePixels[i].a >= alphaThreshold;

                targetMask[i] = isTarget;
                traceMask[i] = isTraced;

                if (isTarget)
                    targetPixelsTotal++;

                if (isTraced)
                {
                    tracePixelCount++;
                    pixelsTraced++;
                    if (isTarget)
                    {
                        pixelsInTarget++;
                    }
                }
            }

            // Coverage should reflect how much of the target path length was completed,
            // not how thick the user's drawn stroke is. To make this thickness-invariant,
            // we consider target pixels "covered" when they are within a tolerance distance
            // from any traced pixel. The tolerance is derived from configured target thickness.
            if (tracePixelCount > 0 && targetPixelsTotal > 0)
            {
                int[] distanceMap = BuildChamferDistanceMap(traceMask, width, height);
                int coverageTolerancePx = Mathf.Clamp(Mathf.CeilToInt(trialData.LineThickness * 0.5f), 1, 64);
                int maxDistanceCost = coverageTolerancePx * 10; // 10 = orthogonal chamfer weight

                for (int i = 0; i < totalPixels; i++)
                {
                    if (targetMask[i] && distanceMap[i] <= maxDistanceCost)
                        targetPixelsCovered++;
                }
            }

            trialData.TargetPixelsTotal = targetPixelsTotal;
            trialData.PixelsTraced = pixelsTraced;
            trialData.PixelsInTarget = pixelsInTarget;
            trialData.TargetPixelsCovered = targetPixelsCovered;
            trialData.Calculate();
        }

        /// <summary>
        /// Build an integer chamfer distance map from traced pixels.
        /// Distances are in cost units where orthogonal step = 10 and diagonal = 14.
        /// </summary>
        private static int[] BuildChamferDistanceMap(bool[] traceMask, int width, int height)
        {
            int total = width * height;
            int[] dist = new int[total];
            const int INF = 1_000_000;

            for (int i = 0; i < total; i++)
                dist[i] = traceMask[i] ? 0 : INF;

            // Forward pass
            for (int y = 0; y < height; y++)
            {
                int row = y * width;
                for (int x = 0; x < width; x++)
                {
                    int idx = row + x;
                    int best = dist[idx];

                    if (x > 0)
                        best = Mathf.Min(best, dist[idx - 1] + 10);
                    if (y > 0)
                        best = Mathf.Min(best, dist[idx - width] + 10);
                    if (x > 0 && y > 0)
                        best = Mathf.Min(best, dist[idx - width - 1] + 14);
                    if (x < width - 1 && y > 0)
                        best = Mathf.Min(best, dist[idx - width + 1] + 14);

                    dist[idx] = best;
                }
            }

            // Backward pass
            for (int y = height - 1; y >= 0; y--)
            {
                int row = y * width;
                for (int x = width - 1; x >= 0; x--)
                {
                    int idx = row + x;
                    int best = dist[idx];

                    if (x < width - 1)
                        best = Mathf.Min(best, dist[idx + 1] + 10);
                    if (y < height - 1)
                        best = Mathf.Min(best, dist[idx + width] + 10);
                    if (x < width - 1 && y < height - 1)
                        best = Mathf.Min(best, dist[idx + width + 1] + 14);
                    if (x > 0 && y < height - 1)
                        best = Mathf.Min(best, dist[idx + width - 1] + 14);

                    dist[idx] = best;
                }
            }

            return dist;
        }

        /// <summary>
        /// Simple nearest-neighbor resample of pixel data.
        /// </summary>
        private static Color32[] ResamplePixels(Texture2D source, int targetWidth, int targetHeight)
        {
            Color32[] sourcePixels = source.GetPixels32();
            Color32[] result = new Color32[targetWidth * targetHeight];

            float xRatio = (float)source.width / targetWidth;
            float yRatio = (float)source.height / targetHeight;

            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    int srcX = Mathf.FloorToInt(x * xRatio);
                    int srcY = Mathf.FloorToInt(y * yRatio);
                    srcX = Mathf.Clamp(srcX, 0, source.width - 1);
                    srcY = Mathf.Clamp(srcY, 0, source.height - 1);
                    result[y * targetWidth + x] = sourcePixels[srcY * source.width + srcX];
                }
            }

            return result;
        }
    }
}
