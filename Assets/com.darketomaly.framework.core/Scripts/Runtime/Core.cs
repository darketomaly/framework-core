using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public static partial class Core
    {
        public static PrefabRegistry PrefabRegistryInstance
        {
            get
            {
                if (!_prefabRegistry)
                {
                    var prefabRegistries = Resources.LoadAll<PrefabRegistry>(string.Empty);
                    _prefabRegistry = prefabRegistries[0];
                }
                
                return _prefabRegistry;
            }
        }

        private static PrefabRegistry _prefabRegistry;
        
        /// <summary>
        /// Safely retrieves an element from an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of the desired element.</typeparam>
        /// <param name="objectArray">The array of objects.</param>
        /// <param name="index">The index of the desired element.</param>
        /// <param name="defaultValue">The default value to return if the element is not found.</param>
        /// <returns>The element at the specified index or the default value.</returns>
        public static T SafeGetAt<T>(this object[] objectArray, int index, T defaultValue = default)
        {
            if (objectArray != null && objectArray.Length > index && index >= 0)
            {
                object obj = objectArray[index];

                if (obj is T obj1)
                {
                    return obj1;
                }
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// Safely retrieves an element from an array of generics.
        /// </summary>
        /// <typeparam name="T">The type of the desired element.</typeparam>
        /// <param name="objectArray">The array of generics.</param>
        /// <param name="index">The index of the desired element.</param>
        /// <param name="defaultValue">The default value to return if the element is not found.</param>
        /// <returns>The element at the specified index or the default value.</returns>
        public static T SafeGetAt<T>(this T[] objectArray, int index, T defaultValue = default)
        {
            if (objectArray != null && objectArray.Length > index && index >= 0)
            {
                object obj = objectArray[index];

                if (obj is T obj1)
                {
                    return obj1;
                }
            }
            
            return defaultValue;
        }
        
        public abstract class Prefs
        {
            private static readonly Dictionary<string, string> Keys = new()
            {
                {Key.LogColor, "627bc4"},
                {Key.ImportantLogColor, "b988d1"},
                {Key.WarningLogColor, "ab953c"},
                {Key.ErrorLogColor, "d9766f"}
            };
            
            public abstract class Key
            {
                public const string LogColor = "LogColor";
                public const string ImportantLogColor = "ImportantLogColor";
                public const string WarningLogColor = "WarningLogColor";
                public const string ErrorLogColor = "ErrorLogColor";
                
                public const string FullTypePath = "FullTypeName";
            }

            #if UNITY_EDITOR
            
            [MenuItem("Tools/Framework/Clear Preferences")]
            public static void Clear()
            {
                EditorPrefs.DeleteKey(Key.LogColor);
                EditorPrefs.DeleteKey(Key.ImportantLogColor);
                EditorPrefs.DeleteKey(Key.WarningLogColor);
                EditorPrefs.DeleteKey(Key.ErrorLogColor);
                EditorPrefs.DeleteKey("FrameworkSetup");
            }
            
            #endif
            
            public static string GetColorString(string key)
            {
                #if !UNITY_EDITOR

                return Keys[key];
                
                #else
                
                return EditorPrefs.GetString(key, Keys[key]);
                
                #endif
            }

            public static Color GetColor(string key)
            {
                // ReSharper disable once RedundantAssignment
                var colorString = Keys[key];
                
                #if UNITY_EDITOR

                colorString = EditorPrefs.GetString(key, Keys[key]);
                
                #endif
                
                ColorUtility.TryParseHtmlString($"#{colorString}", out Color parsedColor);
                
                return parsedColor;
            }
        }
    }
}