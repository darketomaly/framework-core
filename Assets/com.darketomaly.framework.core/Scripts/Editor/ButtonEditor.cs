using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class MethodButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default fields first
        DrawDefaultInspector();

        // Use reflection to find all methods with the [Button] attribute
        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<ButtonAttribute>();
            
            // Fallback to the method's name if no custom label is provided
            string buttonLabel = string.IsNullOrEmpty(attribute.ButtonName) 
                ? ObjectNames.NicifyVariableName(method.Name) 
                : attribute.ButtonName;

            // Restrict to parameterless methods for this basic implementation
            if (method.GetParameters().Length == 0)
            {
                if (GUILayout.Button(buttonLabel))
                {
                    foreach (var t in targets)
                    {
                        method.Invoke(t, null);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Button '{method.Name}' cannot be drawn: Methods with parameters are not supported in this basic implementation.", MessageType.Warning);
            }
        }
    }
}