using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class CoreLogger
    {
        private static string GetNameOrFullName(this System.Type type)
        {
            #if UNITY_EDITOR

            if (EditorPrefs.GetBool(Core.Prefs.Key.FullTypePath, false))
            {
                return type.ToString();
            }
            
            #endif

            return type.Name;
        }

        private static void LogContext<T>(T contextObject, object message, string color, bool isError)
        {
            var type = contextObject.GetType().GetNameOrFullName();
            var msg = $"<color=#{color}>[{type}]</color> {message}";
            var context = contextObject is Object ? (Object)(object)contextObject : null;

            if (isError) Debug.LogError(msg, context);
            else Debug.Log(msg, context);
        }

        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void Log<T>(this T contextObject, object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogContext(contextObject, message, Core.Prefs.GetColorString(Core.Prefs.Key.LogColor), false);
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void Log(object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"<color=#{Core.Prefs.GetColorString(Core.Prefs.Key.LogColor)}>[No Context] </color>{message}");
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void LogWarning<T>(this T contextObject, object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogContext(contextObject, message, Core.Prefs.GetColorString(Core.Prefs.Key.WarningLogColor), false);
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void LogWarning(object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"<color=#{Core.Prefs.GetColorString(Core.Prefs.Key.WarningLogColor)}>[No Context] </color>{message}");
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void LogImportant<T>(this T contextObject, object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogContext(contextObject, message, Core.Prefs.GetColorString(Core.Prefs.Key.ImportantLogColor), false);
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void LogImportant(object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"<color=#{Core.Prefs.GetColorString(Core.Prefs.Key.ImportantLogColor)}>[No Context] </color>{message}");
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void LogError<T>(this T contextObject, object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogContext(contextObject, message, Core.Prefs.GetColorString(Core.Prefs.Key.ErrorLogColor), true);
            #endif
        }
        
        #if UNITY_2022_3_OR_NEWER
        [HideInCallstack]
        #endif
        public static void LogError(object message)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError($"<color=#{Core.Prefs.GetColorString(Core.Prefs.Key.ErrorLogColor)}>[No Context] </color>{message}");
            #endif
        }
    }
}