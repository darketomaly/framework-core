#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static partial class Core
    {
        public static partial class EditorUtility
        {
            public static UnityEditor.Build.NamedBuildTarget GetCurrentNamedBuildTarget()
            {
                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                return UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            }
            
            public static void RemoveFromDefineSymbols(params string[] symbolsToRemove)
            {
                string currentSymbols = PlayerSettings.GetScriptingDefineSymbols(GetCurrentNamedBuildTarget());
                string updatedSymbols = currentSymbols;

                foreach (string symbolToRemove in symbolsToRemove)
                {
                    updatedSymbols = updatedSymbols.Replace(symbolToRemove + ";", "");
                    updatedSymbols = updatedSymbols.Replace(symbolToRemove, "");
                }
                
                Debug.Log($"Symbols should now be: {updatedSymbols}");
            
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, updatedSymbols);
                AssetDatabase.Refresh();
                UnityEditor.EditorUtility.RequestScriptReload();
            }
        }
    }
}

#endif