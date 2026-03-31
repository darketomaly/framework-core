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
            // Load scriptable
            
            var projectConfig = Resources.Load<FrameworkProjectConfig>("Framework/Framework project config");

            if (projectConfig != null)
            {
                projectConfig.Log("Framework config already exists");
                return;
            }

            // If not found, create the necessary folders and asset
            
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
            AssetDatabase.Refresh();

            newConfig.Log($"Created default project config at {AssetPath}");
        }
    }
}