using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Editor script to:
/// 1. Add OVRCameraRig to TracingSession scene and wire up LaserPointer
/// 2. Batch-import all pattern textures as Sprites with Read/Write enabled
/// </summary>
public class SetupVRAndPatterns
{
    [MenuItem("TracingApp/Step 4 - Setup OVRCameraRig in TracingSession")]
    public static void SetupOVRCameraRig()
    {
        // Open TracingSession scene
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/TracingSession.unity", OpenSceneMode.Single);

        // Find and disable the old Main Camera
        var mainCam = GameObject.Find("Main Camera");
        if (mainCam != null)
        {
            mainCam.SetActive(false);
            mainCam.name = "Main Camera (Disabled - replaced by OVRCameraRig)";
            Debug.Log("[SetupVR] Disabled old Main Camera.");
        }

        // Load OVRCameraRig prefab from Meta XR SDK package
        string prefabPath = "Packages/com.meta.xr.sdk.core/Prefabs/OVRCameraRig.prefab";
        var ovrPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (ovrPrefab == null)
        {
            Debug.LogError($"[SetupVR] Could not find OVRCameraRig prefab at: {prefabPath}");
            return;
        }

        // Instantiate OVRCameraRig in the scene
        var ovrInstance = (GameObject)PrefabUtility.InstantiatePrefab(ovrPrefab);
        ovrInstance.name = "OVRCameraRig";
        ovrInstance.transform.position = Vector3.zero;
        ovrInstance.transform.rotation = Quaternion.identity;
        Debug.Log("[SetupVR] OVRCameraRig instantiated in scene.");

        // Find the CenterEyeAnchor camera (this is the gaze camera for our laser pointer)
        var centerEyeAnchor = ovrInstance.transform.Find("TrackingSpace/CenterEyeAnchor");
        Camera centerEyeCamera = null;
        if (centerEyeAnchor != null)
        {
            centerEyeCamera = centerEyeAnchor.GetComponent<Camera>();
            if (centerEyeCamera != null)
            {
                centerEyeCamera.clearFlags = CameraClearFlags.SolidColor;
                centerEyeCamera.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
                Debug.Log("[SetupVR] Found CenterEyeAnchor camera.");
            }
        }

        // Wire up LaserPointer to use the CenterEyeAnchor camera
        var laserPointer = Object.FindFirstObjectByType<TracingApp.Input.LaserPointer>();
        if (laserPointer != null && centerEyeCamera != null)
        {
            laserPointer.GazeCamera = centerEyeCamera;
            laserPointer.UseMouseFallback = true; // Keep mouse fallback for editor testing
            Debug.Log("[SetupVR] LaserPointer.GazeCamera wired to CenterEyeAnchor.");
        }
        else if (laserPointer != null)
        {
            Debug.LogWarning("[SetupVR] LaserPointer found but CenterEyeAnchor camera not found. You may need to wire it manually.");
        }

        // Update the HUD canvas to use the CenterEyeAnchor camera for Screen Space - Camera mode
        // (keeping it as Screen Space - Overlay for now, which works fine in VR)
        var hudCanvas = GameObject.Find("HUDCanvas");
        if (hudCanvas != null)
        {
            var canvas = hudCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Screen Space - Overlay works in VR but may not be ideal
                // For better VR experience, could switch to World Space later
                Debug.Log("[SetupVR] HUDCanvas found. Keeping as Screen Space - Overlay.");
            }
        }

        // Add OVRManager if not already present (OVRCameraRig prefab should have it)
        var ovrManager = ovrInstance.GetComponent<OVRManager>();
        if (ovrManager == null)
        {
            // OVRManager is typically on the OVRCameraRig prefab already
            Debug.LogWarning("[SetupVR] OVRManager not found on OVRCameraRig. It should be part of the prefab.");
        }
        else
        {
            Debug.Log("[SetupVR] OVRManager confirmed on OVRCameraRig.");
        }

        // Save the scene
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SetupVR] TracingSession scene saved with OVRCameraRig!");
    }

    [MenuItem("TracingApp/Step 5 - Import Patterns as Sprites (Read-Write)")]
    public static void ImportPatternsAsSprites()
    {
        string patternsFolder = "Assets/Resources/Patterns";

        // Find all PNG files in the patterns folder
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { patternsFolder });

        if (guids.Length == 0)
        {
            Debug.LogError($"[ImportPatterns] No textures found in {patternsFolder}. Make sure patterns are generated.");
            return;
        }

        Debug.Log($"[ImportPatterns] Found {guids.Length} textures. Updating import settings...");

        int processed = 0;
        int skipped = 0;

        AssetDatabase.StartAssetEditing(); // Batch mode for performance
        try
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                {
                    skipped++;
                    continue;
                }

                bool needsReimport = false;

                // Set texture type to Sprite
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    needsReimport = true;
                }

                // Enable Read/Write
                if (!importer.isReadable)
                {
                    importer.isReadable = true;
                    needsReimport = true;
                }

                // Set sprite mode to Single
                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    needsReimport = true;
                }

                // Disable compression for accurate pixel comparison
                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    needsReimport = true;
                }

                // Set filter mode to Bilinear
                if (importer.filterMode != FilterMode.Bilinear)
                {
                    importer.filterMode = FilterMode.Bilinear;
                    needsReimport = true;
                }

                // Disable mipmaps (not needed for UI/2D sprites)
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    needsReimport = true;
                }

                // Set max texture size to allow full resolution
                if (importer.maxTextureSize < 8192)
                {
                    importer.maxTextureSize = 8192;
                    needsReimport = true;
                }

                if (needsReimport)
                {
                    importer.SaveAndReimport();
                    processed++;
                }
                else
                {
                    skipped++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.Refresh();
        Debug.Log($"[ImportPatterns] Complete! Processed: {processed}, Already correct: {skipped}, Total: {guids.Length}");
    }

    [MenuItem("TracingApp/Run Steps 4 and 5")]
    public static void RunBothSteps()
    {
        SetupOVRCameraRig();
        ImportPatternsAsSprites();
    }
}
