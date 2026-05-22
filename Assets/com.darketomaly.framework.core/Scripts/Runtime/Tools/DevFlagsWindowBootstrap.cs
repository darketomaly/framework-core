#if ODIN_INSPECTOR

using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using Sirenix.OdinInspector.Editor;
#endif


namespace Framework
{
    #if UNITY_EDITOR
    
    public class DevFlagsLauncher : EditorWindow
    {
        // Add MenuItem to any child class of DevFlagsWindowBase
        
        [MenuItem("Tools/Framework/Development Window", priority = 100)]
        private static void OpenDevFlagsWindow()
        {
            var windowType = TypeCache.GetTypesDerivedFrom<DevFlagsWindow>().FirstOrDefault(t => !t.IsAbstract && !t.IsGenericType);

            if (windowType == null)
            {
                Debug.LogError("No DevFlagsWindowBase usage found");
                return;
            }

            GetWindow(windowType, false, "Development Window");
        }
    }
    
    #endif
    
    /// <summary>
    /// The <see cref="DevFlagsLauncher"/> automatically handles the MenuItem under Tools/Framework/Development Window.
    /// <example>Implement bool fields this way:
    /// <list>
    ///   <item>- Selected values persist when you close and open the window.</item>
    ///   <item>- Selected values are respected when you start the game on fullscreen, with the editor window not valid.</item>
    /// </list>
    /// <code>
    /// [ShowInInspector]
    /// public static bool UnlimitedMana 
    /// { 
    ///     get => GetBool(); 
    ///     set => SetBool(value); 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public abstract class DevFlagsWindow
        #if UNITY_EDITOR
        : OdinEditorWindow
        #endif
    {
        // --- Setup ---
        
        // Cache fields, so EditorPrefs.GetBool is not called constantly
        // Using editor prefs allow me to access the last valid value, in case the editor window is not valid
        // For example, when starting the game on fullscreen
        
        private static readonly Dictionary<string, bool> CachedBools = new();
        private static readonly Dictionary<string, Object> CachedAssets = new();
    
        protected static bool GetBool(bool defaultValue = false, [System.Runtime.CompilerServices.CallerMemberName] string key = null)
        {
            if (!CachedBools.TryGetValue(key, out var value))
            {
                #if UNITY_EDITOR
                
                value = EditorPrefs.GetBool(key, defaultValue);
                
                #else
                
                value = defaultValue;
                    
                #endif
                
                CachedBools[key] = value;
            }
            
            return value;
        }
    
        protected static void SetBool(bool value, [System.Runtime.CompilerServices.CallerMemberName] string key = null)
        {
            CachedBools[key] = value;
            
            #if UNITY_EDITOR
            
            EditorPrefs.SetBool(key, value);
            Debug.Log($"<color=yellow>[DevFlagsWindow]</color> Setting {key}: {value}");
            
            #endif
        }

        protected static T GetAsset<T>([System.Runtime.CompilerServices.CallerMemberName] string key = null) where T : Object
        {
            if (!CachedAssets.TryGetValue(key, out var value))
            {
                #if UNITY_EDITOR
                
                var assetPath = EditorPrefs.GetString(key);

                if (!string.IsNullOrEmpty(assetPath))
                {
                    value = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                }

                #else
                
                value = null;
                    
                #endif
                
                CachedAssets[key] = value;
            }

            return value as T;
        }

        protected static void SetAsset(Object value, [System.Runtime.CompilerServices.CallerMemberName] string key = null)
        {
            CachedAssets[key] = value;
            
            #if UNITY_EDITOR
            
            var path = AssetDatabase.GetAssetPath(value);
            EditorPrefs.SetString(key, path);
            Debug.Log($"<color=yellow>[DevFlagsWindow]</color> Setting {key}: {value.name}");
            
            #endif
        }
        
        // --- Predefined buttons ---

        [TitleGroup("Framework")]
        
        #if UNITY_EDITOR
        
        [PropertyOrder(999)]
        [Button]
        private void Recompile()
        {
            Debug.Log("<color=yellow>[DevFlagsWindow]</color> Requested recompilation");
            CompilationPipeline.RequestScriptCompilation();
        }
        
        #endif
    
        [PropertyOrder(999)]
        [Button]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("<color=yellow>[DevFlagsWindow]</color> Cleared preferences");
        }
    }
}

#endif