#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Assets.Scripts.Utils
{
    public class ServerBuild
    {
        [MenuItem("Build/Build Headless Server")]
        public static void BuildServer()
        {
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = new[]
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Lobby.unity",
                "Assets/Scenes/Campaign.unity", 
            },
                locationPathName = "Build/Server/HeadlessServer.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            };

            var previousSubtarget = EditorUserBuildSettings.standaloneBuildSubtarget;
            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Server;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            EditorUserBuildSettings.standaloneBuildSubtarget = previousSubtarget;

            if (report.summary.result == BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log("Headless server build succeeded.");
            }
            else
            {
                UnityEngine.Debug.LogError("Headless server build failed.");
            }
        }
    }
}
#endif