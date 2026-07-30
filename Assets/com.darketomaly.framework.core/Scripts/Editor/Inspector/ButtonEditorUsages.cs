using UnityEngine;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class MonoBehaviourButtonEditor : ButtonEditorBase { }

    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ScriptableObjectButtonEditor : ButtonEditorBase { }
}