using UnityEngine;
using UnityEngine.UI;

namespace TracingApp.UI
{
    /// <summary>
    /// Flashing red recording indicator that appears when tracing is active.
    /// </summary>
    public class RecordingIndicator : MonoBehaviour
    {
        [Header("Visual")]
        public Image IndicatorImage;
        public Color IndicatorColor = Color.red;

        [Header("Flash Settings")]
        public float FlashRate = 2f; // Flashes per second
        public float MinAlpha = 0.2f;
        public float MaxAlpha = 1f;

        [Header("Label")]
        public TMPro.TextMeshProUGUI RecordingLabel;

        private bool _isActive;
        private float _flashTimer;

        private void Start()
        {
            SetActive(false);
        }

        private void Update()
        {
            if (!_isActive) return;

            _flashTimer += Time.deltaTime * FlashRate * 2f * Mathf.PI;
            float alpha = Mathf.Lerp(MinAlpha, MaxAlpha, (Mathf.Sin(_flashTimer) + 1f) / 2f);

            if (IndicatorImage != null)
            {
                Color c = IndicatorColor;
                c.a = alpha;
                IndicatorImage.color = c;
            }
        }

        /// <summary>
        /// Show or hide the recording indicator.
        /// </summary>
        public void SetActive(bool active)
        {
            _isActive = active;
            _flashTimer = 0f;

            if (IndicatorImage != null)
                IndicatorImage.gameObject.SetActive(active);

            if (RecordingLabel != null)
                RecordingLabel.gameObject.SetActive(active);
        }
    }
}
