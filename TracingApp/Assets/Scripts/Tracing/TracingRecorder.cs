using UnityEngine;
using UnityEngine.UI;

namespace TracingApp.Tracing
{
    /// <summary>
    /// Records the user's trace on a texture by painting pixels where the laser hits.
    /// Maintains a trace texture that can be compared against the pattern for scoring.
    /// </summary>
    public class TracingRecorder : MonoBehaviour
    {
        [Header("Trace Settings")]
        [Tooltip("Color of the burn trail")]
        public Color TraceColor = new Color(1f, 0f, 0f, 0.8f);

        [Tooltip("Radius of the trace brush in pixels")]
        public int BrushRadius = 3;

        [Header("Optional Display")]
        [Tooltip("RawImage used to display the live trace texture (typically TraceOverlay)")]
        public RawImage TraceDisplay;

        // State
        public bool IsRecording { get; private set; }

        private Texture2D _traceTexture;
        private int _textureWidth;
        private int _textureHeight;
        private Color32[] _tracePixels;
        private bool _isDirty;

        /// <summary>
        /// Initialize the trace texture to match the pattern dimensions.
        /// </summary>
        public void Initialize(int width, int height)
        {
            _textureWidth = width;
            _textureHeight = height;

            // Create a transparent texture for the trace
            _traceTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            _traceTexture.filterMode = FilterMode.Bilinear;

            // Clear to transparent
            _tracePixels = new Color32[width * height];
            Color32 clear = new Color32(0, 0, 0, 0);
            for (int i = 0; i < _tracePixels.Length; i++)
                _tracePixels[i] = clear;

            _traceTexture.SetPixels32(_tracePixels);
            _traceTexture.Apply();
            _isDirty = false;

            if (TraceDisplay == null)
            {
                var traceOverlay = GameObject.Find("TraceOverlay");
                if (traceOverlay != null)
                    TraceDisplay = traceOverlay.GetComponent<RawImage>();
            }

            if (TraceDisplay != null)
            {
                TraceDisplay.texture = _traceTexture;
                if (TraceDisplay.color.a <= 0f)
                    TraceDisplay.color = new Color(1f, 1f, 1f, 0.8f);
            }

            Debug.Log($"[TracingRecorder] Initialized {width}x{height} trace texture.");
        }

        /// <summary>
        /// Start recording trace input.
        /// </summary>
        public void StartRecording()
        {
            IsRecording = true;
        }

        /// <summary>
        /// Stop recording trace input.
        /// </summary>
        public void StopRecording()
        {
            IsRecording = false;
            ApplyChanges();
        }

        /// <summary>
        /// Paint a point on the trace texture at the given UV coordinates.
        /// UV coordinates are 0-1 range where (0,0) is bottom-left.
        /// </summary>
        public void PaintAtUV(Vector2 uv)
        {
            if (!IsRecording) return;
            if (_traceTexture == null) return;

            int centerX = Mathf.FloorToInt(uv.x * _textureWidth);
            int centerY = Mathf.FloorToInt(uv.y * _textureHeight);

            PaintCircle(centerX, centerY, BrushRadius);
            _isDirty = true;
        }

        /// <summary>
        /// Paint a line between two UV points (for smooth strokes).
        /// </summary>
        public void PaintLineUV(Vector2 fromUV, Vector2 toUV)
        {
            if (!IsRecording) return;
            if (_traceTexture == null) return;

            int x0 = Mathf.FloorToInt(fromUV.x * _textureWidth);
            int y0 = Mathf.FloorToInt(fromUV.y * _textureHeight);
            int x1 = Mathf.FloorToInt(toUV.x * _textureWidth);
            int y1 = Mathf.FloorToInt(toUV.y * _textureHeight);

            // Bresenham's line algorithm
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                PaintCircle(x0, y0, BrushRadius);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            _isDirty = true;
        }

        /// <summary>
        /// Paint a filled circle at the given pixel coordinates.
        /// </summary>
        private void PaintCircle(int centerX, int centerY, int radius)
        {
            Color32 color = new Color32(
                (byte)(TraceColor.r * 255),
                (byte)(TraceColor.g * 255),
                (byte)(TraceColor.b * 255),
                (byte)(TraceColor.a * 255)
            );

            int rSq = radius * radius;

            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    if (dx * dx + dy * dy <= rSq)
                    {
                        int px = centerX + dx;
                        int py = centerY + dy;

                        if (px >= 0 && px < _textureWidth && py >= 0 && py < _textureHeight)
                        {
                            int idx = py * _textureWidth + px;
                            // Alpha blend
                            if (_tracePixels[idx].a < color.a)
                                _tracePixels[idx] = color;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apply pending pixel changes to the texture. Call periodically for visual updates.
        /// </summary>
        public void ApplyChanges()
        {
            if (!_isDirty || _traceTexture == null) return;

            _traceTexture.SetPixels32(_tracePixels);
            _traceTexture.Apply();
            _isDirty = false;
        }

        private void LateUpdate()
        {
            // Apply changes each frame for visual feedback
            if (IsRecording && _isDirty)
            {
                ApplyChanges();
            }
        }

        /// <summary>
        /// Get the trace texture for scoring.
        /// </summary>
        public Texture2D GetTraceTexture()
        {
            ApplyChanges();
            return _traceTexture;
        }

        /// <summary>
        /// Create a composite texture showing the pattern with the trace overlay.
        /// Used for screenshots.
        /// </summary>
        public Texture2D GetCompositeTexture(Texture2D patternTexture)
        {
            if (patternTexture == null || _traceTexture == null) return _traceTexture;

            int width = patternTexture.width;
            int height = patternTexture.height;

            Texture2D composite = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color32[] patternPixels = patternTexture.GetPixels32();
            Color32[] tracePixels = _traceTexture.GetPixels32();
            Color32[] result = new Color32[width * height];

            // Black background
            Color32 bgColor = new Color32(0, 0, 0, 255);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = bgColor;

                // Draw pattern in white
                if (patternPixels[i].a > 128)
                {
                    result[i] = new Color32(255, 255, 255, 255);
                }

                // Overlay trace in red
                if (i < tracePixels.Length && tracePixels[i].a > 128)
                {
                    // Blend trace color over current pixel
                    float alpha = tracePixels[i].a / 255f;
                    result[i] = new Color32(
                        (byte)Mathf.Lerp(result[i].r, tracePixels[i].r, alpha),
                        (byte)Mathf.Lerp(result[i].g, tracePixels[i].g, alpha),
                        (byte)Mathf.Lerp(result[i].b, tracePixels[i].b, alpha),
                        255
                    );
                }
            }

            composite.SetPixels32(result);
            composite.Apply();
            return composite;
        }

        private void OnDestroy()
        {
            if (_traceTexture != null)
            {
                if (Application.isPlaying)
                    Destroy(_traceTexture);
                else
                    DestroyImmediate(_traceTexture);
            }
        }
    }
}
