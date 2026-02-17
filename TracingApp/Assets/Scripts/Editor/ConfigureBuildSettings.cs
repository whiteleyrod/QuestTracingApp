using UnityEditor;
using System.Collections.Generic;

public class ConfigureBuildSettings
{
    [MenuItem("TracingApp/Configure Build Settings")]
    public static void Execute()
    {
        var scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Settings.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/TracingSession.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Results.unity", true),
        };

        EditorBuildSettings.scenes = scenes.ToArray();
        UnityEngine.Debug.Log("[ConfigureBuildSettings] Build settings configured: MainMenu(0), Settings(1), TracingSession(2), Results(3)");
    }
}
