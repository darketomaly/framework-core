using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class ButtonDrawer
    {
        // Hardcoded appearance
        private const float ButtonHeight = 30f;
        private static readonly Color ButtonColor = new Color(102f / 255f, 191.2f / 255f, 255f / 255f);

        private class ButtonEntry
        {
            public MethodInfo Method;
            public ButtonAttribute Attribute;
            public ParameterInfo[] Parameters;
            public object[] ParamValues;
            public bool Supported;
        }

        private static readonly HashSet<Type> SupportedParamTypes = new HashSet<Type>
        {
            typeof(int), typeof(float), typeof(bool), typeof(string)
        };

        private List<ButtonEntry> _entries = new List<ButtonEntry>();
        private GUIStyle _buttonStyle;

        public void Init(object target)
        {
            _entries.Clear();
            if (target == null) return;

            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                bool supported = parameters.All(p => SupportedParamTypes.Contains(p.ParameterType));

                _entries.Add(new ButtonEntry
                {
                    Method = method,
                    Attribute = method.GetCustomAttribute<ButtonAttribute>(),
                    Parameters = parameters,
                    ParamValues = parameters.Select(p => GetDefault(p.ParameterType)).ToArray(),
                    Supported = supported
                });
            }
        }

        private GUIStyle GetButtonStyle()
        {
            // GUIStyle can't be built in a field initializer / before GUI runs, so lazily create it
            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold,
                    fixedHeight = ButtonHeight
                };
            }
            return _buttonStyle;
        }

        public void Draw(UnityEngine.Object[] targets)
        {
            if (_entries.Count == 0) return;

            GUILayout.Space(8);

            var style = GetButtonStyle();
            var previousColor = GUI.backgroundColor;

            foreach (var entry in _entries)
            {
                if (!entry.Supported)
                {
                    EditorGUILayout.HelpBox(
                        $"[Button] on '{entry.Method.Name}' has unsupported parameter types.",
                        MessageType.Warning);
                    continue;
                }

                var label = string.IsNullOrEmpty(entry.Attribute.Label)
                    ? ObjectNames.NicifyVariableName(entry.Method.Name)
                    : entry.Attribute.Label;

                for (int i = 0; i < entry.Parameters.Length; i++)
                {
                    var p = entry.Parameters[i];
                    var fieldLabel = ObjectNames.NicifyVariableName(p.Name);

                    if (p.ParameterType == typeof(int))
                        entry.ParamValues[i] = EditorGUILayout.IntField(fieldLabel, (int)entry.ParamValues[i]);
                    else if (p.ParameterType == typeof(float))
                        entry.ParamValues[i] = EditorGUILayout.FloatField(fieldLabel, (float)entry.ParamValues[i]);
                    else if (p.ParameterType == typeof(bool))
                        entry.ParamValues[i] = EditorGUILayout.Toggle(fieldLabel, (bool)entry.ParamValues[i]);
                    else if (p.ParameterType == typeof(string))
                        entry.ParamValues[i] = EditorGUILayout.TextField(fieldLabel, (string)entry.ParamValues[i]);
                }

                GUI.backgroundColor = ButtonColor;
                bool pressed = GUILayout.Button(label, style);
                GUI.backgroundColor = previousColor;

                if (pressed)
                {
                    foreach (var t in targets)
                        entry.Method.Invoke(t, entry.ParamValues);
                }

                if (entry.Parameters.Length > 0) GUILayout.Space(4);
            }
        }

        private static object GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}