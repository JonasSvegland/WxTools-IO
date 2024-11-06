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

            // Kontrollera och �ndra API-niv�n om den inte �r satt till .NET Framework
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

            if (PlayerSettings.GetApiCompatibilityLevel(group) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_Unity_4_8);
                Debug.Log("API Compatibility Level �ndrad till .NET Framework");

                // F�r att s�kerst�lla att Unity tar h�nsyn till f�r�ndringen direkt
                AssetDatabase.Refresh();
            }

        }
    }
}