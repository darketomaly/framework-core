using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class FrameworkAutoConfig
    {
        private const string ResourcesFolder = "Assets/Resources";
        private const string FrameworkSubFolder = "Framework";
        private const string AssetPath = "Assets/Resources/Framework/Framework.asset";

        static FrameworkAutoConfig()
        {
            // First, try to load it using Resources (this is what you'll use at runtime)
            FrameworkProjectConfig projectConfig = Resources.Load<FrameworkProjectConfig>("Framework/Framework");

            if (projectConfig != null)
            {
                projectConfig.Log("Framework config already exists");
                return;
            }

            // If not found via Resources.Load, create the necessary folders and asset
            
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

            Debug.Log($"[Framework] Created default project config at: {AssetPath}");
        }
    }
}