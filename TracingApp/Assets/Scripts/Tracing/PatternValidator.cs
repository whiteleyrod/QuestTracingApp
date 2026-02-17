using UnityEngine;

namespace TracingApp.Tracing
{
    /// <summary>
    /// Provides real-time validation of trace position against the pattern.
    /// Can be used for immediate visual/audio feedback.
    /// </summary>
    public class PatternValidator : MonoBehaviour
    {
        private Texture2D _patternTexture;
        private Color32[] _patternPixels;
        private int _width;
        private int _height;
        private byte _alphaThreshold = 128;

        /// <summary>
        /// Initialize with the current pattern texture.
        /// </summary>
        public void Initialize(Texture2D patternTexture)
        {
            _patternTexture = patternTexture;
            if (patternTexture != null && patternTexture.isReadable)
            {
                _patternPixels = patternTexture.GetPixels32();
                _width = patternTexture.width;
                _height = patternTexture.height;
            }
            else
            {
                Debug.LogWarning("[PatternValidator] Pattern texture is null or not readable.");
                _patternPixels = null;
            }
        }

        /// <summary>
        /// Check if a UV coordinate is on the target pattern.
        /// </summary>
        public bool IsOnTarget(Vector2 uv)
        {
            if (_patternPixels == null) return false;

            int x = Mathf.FloorToInt(uv.x * _width);
            int y = Mathf.FloorToInt(uv.y * _height);

            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return false;

            int idx = y * _width + x;
            return _patternPixels[idx].a >= _alphaThreshold;
        }
    }
}
