#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomEditor(typeof(FrameworkProjectConfig))]
    public class FrameworkProjectConfigEditor : UnityEditor.Editor
    {
        private static readonly BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw all core fields normally
            DrawPropertiesExcluding(serializedObject, "m_Modules");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Optional Modules", EditorStyles.boldLabel);

            var config = (FrameworkProjectConfig)target;
            var modules = config.Modules;

            if (modules.Count == 0)
            {
                EditorGUILayout.HelpBox("No optional modules installed yet.", MessageType.Info);
            }
            else
            {
                foreach (var module in modules)
                {
                    if (module == null) continue;

                    string title = ObjectNames.NicifyVariableName(
                        module.GetType().Name.Replace("ModuleData", "").Replace("Data", ""));

                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;

                        DrawObjectFieldsWithReflection(module);

                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.Space(8);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawObjectFieldsWithReflection(object obj)
        {
            if (obj == null) return;

            Type type = obj.GetType();

            // Get all serialized fields (public + private with [SerializeField])
            var fields = type.GetFields(fieldFlags);

            foreach (var field in fields)
            {
                // Skip fields that Unity wouldn't serialize 
                if (field.IsStatic) continue;
                if (field.IsInitOnly) continue; // readonly

                // Check for [SerializeField] or public field
                bool shouldSerialize = field.IsPublic || 
                                       field.GetCustomAttribute<SerializeField>() != null;

                if (!shouldSerialize) continue;

                object value = field.GetValue(obj);
                GUIContent label = new GUIContent(ObjectNames.NicifyVariableName(field.Name));

                // Draw simple types (string, int, float, bool, etc.)
                if (field.FieldType == typeof(string))
                {
                    string newValue = EditorGUILayout.TextField(label, (string)value);
                    if (newValue != (string)value)
                    {
                        field.SetValue(obj, newValue);
                        EditorUtility.SetDirty(target);
                    }
                }
                else if (field.FieldType == typeof(int))
                {
                    int newValue = EditorGUILayout.IntField(label, (int)value);
                    if (newValue != (int)value)
                    {
                        field.SetValue(obj, newValue);
                        EditorUtility.SetDirty(target);
                    }
                }
                // Add more types as needed (float, bool, Vector3, etc.)
                else if (field.FieldType == typeof(float))
                {
                    float newValue = EditorGUILayout.FloatField(label, (float)value);
                    if (!Mathf.Approximately(newValue, (float)value))
                    {
                        field.SetValue(obj, newValue);
                        EditorUtility.SetDirty(target);
                    }
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool newValue = EditorGUILayout.Toggle(label, (bool)value);
                    if (newValue != (bool)value)
                    {
                        field.SetValue(obj, newValue);
                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    // Fallback for complex types or unsupported
                    EditorGUILayout.LabelField(label, value?.ToString() ?? "null");
                }
            }
        }
    }
}
#endif