using UnityEditor;
using UnityEngine;

public class EnableOculusXRForPC
{
    [MenuItem("TracingApp/Enable Oculus XR for PC (Quest Link)")]
    public static void Execute()
    {
        // Check current XR settings for Standalone (PC)
        var generalSettings = UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget
            .XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);

        if (generalSettings == null)
        {
            Debug.LogWarning("[XR Setup] Could not find XR General Settings for Standalone. " +
                "Please go to Edit → Project Settings → XR Plug-in Management → Windows tab → check Oculus manually.");
            return;
        }

        var manager = generalSettings.Manager;
        if (manager == null)
        {
            Debug.LogWarning("[XR Setup] XR Manager is null for Standalone. " +
                "Please enable it in Edit → Project Settings → XR Plug-in Management.");
            return;
        }

        Debug.Log("[XR Setup] Current Standalone XR loaders:");
        foreach (var loader in manager.activeLoaders)
        {
            Debug.Log($"  - {loader.GetType().Name}");
        }

        Debug.Log("[XR Setup] To enable Quest Link testing:");
        Debug.Log("  1. Edit → Project Settings → XR Plug-in Management");
        Debug.Log("  2. Click the Windows/PC tab (monitor icon)");
        Debug.Log("  3. Check 'Oculus'");
        Debug.Log("  4. Press Play in the editor with Quest Link active");
    }
}
