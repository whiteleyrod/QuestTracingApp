using System.IO;
using UnityEngine;

namespace TracingApp.Data
{
    /// <summary>
    /// Captures screenshots of the pattern canvas with burn trail overlay.
    /// </summary>
    public class ScreenshotCapture : MonoBehaviour
    {
        /// <summary>
        /// Capture a RenderTexture to a PNG file.
        /// </summary>
        public static string CaptureRenderTexture(RenderTexture renderTexture, string userID,
            string sessionTimestamp, int trialNumber)
        {
            if (renderTexture == null)
            {
                Debug.LogWarning("[ScreenshotCapture] RenderTexture is null, cannot capture.");
                return "";
            }

            string dir = CSVWriter.GetScreenshotDirectory(userID, sessionTimestamp);
            Directory.CreateDirectory(dir);

            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = $"{userID}_trial{trialNumber}_{timestamp}.png";
            string filepath = Path.Combine(dir, filename);

            // Read from RenderTexture
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            RenderTexture.active = previous;

            // Save as PNG
            byte[] pngData = tex.EncodeToPNG();
            File.WriteAllBytes(filepath, pngData);

            // Cleanup
            if (Application.isPlaying)
                Destroy(tex);
            else
                DestroyImmediate(tex);

            Debug.Log($"[ScreenshotCapture] Screenshot saved to: {filepath}");
            return filename;
        }

        /// <summary>
        /// Capture a Texture2D directly to a PNG file.
        /// </summary>
        public static string CaptureTexture(Texture2D texture, string userID,
            string sessionTimestamp, int trialNumber)
        {
            if (texture == null)
            {
                Debug.LogWarning("[ScreenshotCapture] Texture is null, cannot capture.");
                return "";
            }

            string dir = CSVWriter.GetScreenshotDirectory(userID, sessionTimestamp);
            Directory.CreateDirectory(dir);

            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = $"{userID}_trial{trialNumber}_{timestamp}.png";
            string filepath = Path.Combine(dir, filename);

            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(filepath, pngData);

            Debug.Log($"[ScreenshotCapture] Screenshot saved to: {filepath}");
            return filename;
        }
    }
}
