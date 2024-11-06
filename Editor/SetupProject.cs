using UnityEngine;
using UnityEditor;

namespace WxTools.IO
{
    [InitializeOnLoad]
    public class SetupProject
    {
        [MenuItem("WxTools/Setup project for serial communication")]
        private static void Setup()
        {
            /*  const string projectSettingsAssetPath = "ProjectSettings/ProjectSettings.asset";
              SerializedObject projectSettings = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(projectSettingsAssetPath)[0]);
              SerializedProperty mode = projectSettings.FindProperty("apiCompatibilityLevel");

              if (mode.intValue != (int)ApiCompatibilityLevel.NET_Unity_4_8)
              {
                  Debug.Log("Modifying project settings");
                  Debug.Log("Changing ApiCompatibilityLevel to .NET Framework");
                  mode.intValue = (int)ApiCompatibilityLevel.NET_Unity_4_8;
                  projectSettings.ApplyModifiedProperties();
                  CompilationPipeline.RequestScriptCompilation();
              }
              else
                  Debug.Log("No need to modify project settings");*/

            // Kontrollera och ändra API-nivån om den inte är satt till .NET Framework
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

            if (PlayerSettings.GetApiCompatibilityLevel(group) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_Unity_4_8);
                Debug.Log("API Compatibility Level ändrad till .NET Framework");

                // För att säkerställa att Unity tar hänsyn till förändringen direkt
                AssetDatabase.Refresh();
            }

        }
    }
}