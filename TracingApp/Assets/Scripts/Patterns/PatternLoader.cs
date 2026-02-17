using UnityEngine;
using TracingApp.Data;

namespace TracingApp.Patterns
{
    /// <summary>
    /// Loads pattern textures from Resources/Patterns based on settings.
    /// Pattern naming: scale_{w}x{h}_width_{level}_{patternKey}.png
    /// </summary>
    public static class PatternLoader
    {
        private const string PATTERNS_PATH = "Patterns/";

        /// <summary>
        /// Build the resource path for a pattern (without extension).
        /// </summary>
        public static string GetPatternResourcePath(string patternKey, int widthLevel, string scaleString)
        {
            return $"{PATTERNS_PATH}scale_{scaleString}_width_{widthLevel}_{patternKey}";
        }

        /// <summary>
        /// Load a pattern texture from Resources.
        /// </summary>
        public static Texture2D LoadPatternTexture(string patternKey, int widthLevel, string scaleString)
        {
            string path = GetPatternResourcePath(patternKey, widthLevel, scaleString);
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex == null)
            {
                Debug.LogWarning($"[PatternLoader] Could not load pattern at: {path}");
            }
            return tex;
        }

        /// <summary>
        /// Load a pattern as a Sprite for UI display.
        /// </summary>
        public static Sprite LoadPatternSprite(string patternKey, int widthLevel, string scaleString)
        {
            Texture2D tex = LoadPatternTexture(patternKey, widthLevel, scaleString);
            if (tex == null) return null;

            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

        /// <summary>
        /// Load a pattern using SessionSettings to determine width and scale.
        /// </summary>
        public static Texture2D LoadPatternTexture(PatternInfo pattern, SessionSettings settings)
        {
            return LoadPatternTexture(pattern.Key, settings.LineThickness, settings.GetScaleString());
        }

        /// <summary>
        /// Load a pattern sprite using SessionSettings.
        /// </summary>
        public static Sprite LoadPatternSprite(PatternInfo pattern, SessionSettings settings)
        {
            return LoadPatternSprite(pattern.Key, settings.LineThickness, settings.GetScaleString());
        }

        /// <summary>
        /// Count the number of white (target) pixels in a pattern texture.
        /// Used for coverage calculation.
        /// </summary>
        public static int CountTargetPixels(Texture2D texture, byte alphaThreshold = 128)
        {
            if (texture == null) return 0;

            // Need readable texture
            if (!texture.isReadable)
            {
                Debug.LogWarning("[PatternLoader] Texture is not readable. Enable Read/Write in import settings.");
                return 0;
            }

            Color32[] pixels = texture.GetPixels32();
            int count = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a >= alphaThreshold)
                    count++;
            }
            return count;
        }
    }
}
