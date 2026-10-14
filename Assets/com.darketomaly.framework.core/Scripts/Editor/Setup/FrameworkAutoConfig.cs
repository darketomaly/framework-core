using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class FrameworkAutoConfig
    {
        private const string ResourcesFolder = "Assets/Resources";
        private const string FrameworkSubFolder = "Framework";
        private const string AssetPath = "Assets/Resources/Framework/Framework project config.asset";

        static FrameworkAutoConfig()
        {
            EditorApplication.delayCall += EnsureConfigExists;
        }

        private static void EnsureConfigExists()
        {
            var config = AssetDatabase.LoadAssetAtPath<FrameworkProjectConfig>(AssetPath);
            
            if (config || Resources.Load<FrameworkProjectConfig>("Framework/Framework project config"))
            {
                return;
            }

            var configFilePath = Path.Combine(Application.dataPath, "Resources", FrameworkSubFolder, "Framework project config.asset");
            
            if (File.Exists(configFilePath) || AssetDatabase.LoadMainAssetAtPath(AssetPath))
            {
                Debug.LogError($"[FrameworkAutoConfig] Found an existing asset at {AssetPath}, but it could not be loaded as a {nameof(FrameworkProjectConfig)}. It was left untouched.");
                return;
            }
            
            // --- If not found, create the necessary folders and asset ----

            // Create Resources folder if it doesn't exist
            
            if (!AssetDatabase.IsValidFolder(ResourcesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            
            // Create Framework subfolder inside Resources

            var frameworkFolderPath = $"{ResourcesFolder}/{FrameworkSubFolder}";
            
            if (!AssetDatabase.IsValidFolder(frameworkFolderPath))
            {
                AssetDatabase.CreateFolder(ResourcesFolder, FrameworkSubFolder);
            }
            
            // Create the ScriptableObject asset

            var newConfig = ScriptableObject.CreateInstance<FrameworkProjectConfig>();
            AssetDatabase.CreateAsset(newConfig, AssetPath);
            AssetDatabase.SaveAssets();

            newConfig.Log($"Created default project config at {AssetPath}");
        }
    }
}