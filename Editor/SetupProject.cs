using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace WxTools.IO
{ 
    public class SetupProject
    {
        [MenuItem("WxTools/Setup project for serial communication")]
        private static void Setup()
        {
            const string projectSettingsAssetPath = "ProjectSettings/ProjectSettings.asset";
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
                Debug.Log("No need to modify project settings");

        }
    }
}