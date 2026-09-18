using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.EditorTools
{
    [InitializeOnLoad]
    internal static class ButtonEditor
    {
        private const BindingFlags MethodFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        static ButtonEditor()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI += DrawButtons;
        }

        private static void DrawButtons(UnityEditor.Editor editor)
        {
            if (!(editor.target is MonoBehaviour))
                return;

            var methods = editor.target.GetType()
                .GetMethods(MethodFlags)
                .Where(method => method.GetCustomAttribute<ButtonAttribute>() != null)
                .Where(method => !method.IsSpecialName && method.GetParameters().Length == 0)
                .ToArray();

            if (methods.Length == 0)
                return;

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            foreach (var method in methods)
            {
                var button = method.GetCustomAttribute<ButtonAttribute>();
                var label = string.IsNullOrEmpty(button.Label)
                    ? ObjectNames.NicifyVariableName(method.Name)
                    : button.Label;

                if (!GUILayout.Button(label))
                    continue;

                foreach (var target in editor.targets)
                {
                    var monoBehaviour = target as MonoBehaviour;
                    if (monoBehaviour == null)
                        continue;

                    Undo.RecordObject(monoBehaviour, label);
                    method.Invoke(monoBehaviour, null);
                    EditorUtility.SetDirty(monoBehaviour);
                }
            }
        }
    }
}
