using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// ButtonWindow features:
// - Searches for button implementations when entering play mode or when Refresh is clicked.
// - Groups buttons by their parent type, including generic parent types; each group can be minimized.
// - Supports searching by method or gameObject name.
// - Displays the gameObject name next to the method name in gray inside the button.
// - Displays each method on its own row with a fixed-width select button.
// - Shrinks the action button to keep the select button visible at narrow window widths.
// - Prevents horizontal scrolling while preserving vertical scrolling.
// - Uses the full available width without a vertical scrollbar and shifts the select button left when one is needed.
public sealed class ButtonWindow : EditorWindow
{
    private readonly List<ButtonTarget> buttonTargets = new List<ButtonTarget>();
    private readonly Dictionary<Type, bool> groupFoldouts = new Dictionary<Type, bool>();
    private VisualTreeAsset m_ButtonItemAsset;
    private ScrollView m_ButtonScrollView;
    private TextField m_SearchField;

    [MenuItem("Tools/Framework/Dark Buttons")]
    private static void Open()
    {
        GetWindow<ButtonWindow>("Dark Buttons");
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    public void CreateGUI()
    {
        var visualTree = LoadAssetNextToScript<VisualTreeAsset>("ButtonWindow.uxml");
        var styleSheet = LoadAssetNextToScript<StyleSheet>("ButtonWindow.uss");
        m_ButtonItemAsset = LoadAssetNextToScript<VisualTreeAsset>("ButtonWindowItem.uxml");

        if (visualTree == null || styleSheet == null || m_ButtonItemAsset == null)
        {
            return;
        }

        visualTree.CloneTree(rootVisualElement);
        rootVisualElement.styleSheets.Add(styleSheet);

        m_ButtonScrollView = rootVisualElement.Q<ScrollView>("button-scroll-view");
        m_SearchField = rootVisualElement.Q<TextField>("search-field");
        var refreshButton = rootVisualElement.Q<Button>("refresh-button");

        refreshButton.clicked += Refresh;
        m_SearchField.RegisterValueChangedCallback(_ => RebuildButtonView());

        Refresh();
    }

    private T LoadAssetNextToScript<T>(string fileName) where T : UnityEngine.Object
    {
        var script = MonoScript.FromScriptableObject(this);
        var scriptPath = AssetDatabase.GetAssetPath(script);
        var scriptDirectory = Path.GetDirectoryName(scriptPath);

        if (string.IsNullOrEmpty(scriptDirectory))
        {
            Debug.LogError($"Could not determine the asset directory for {GetType().Name}.");
            return null;
        }

        var assetPath = Path.Combine(scriptDirectory, fileName).Replace('\\', '/');
        var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            Debug.LogError($"Could not load {typeof(T).Name} at '{assetPath}'.", this);
        }

        return asset;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        buttonTargets.Clear();

        var components = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
            .Where(component => component != null)
            .Where(component => component.gameObject != null)
            .Where(component => component.gameObject.scene.IsValid())
            .Where(component => !EditorUtility.IsPersistent(component))
            .OrderBy(component => component.gameObject.scene.name)
            .ThenBy(component => component.transform.GetHierarchyPath())
            .ToList();

        foreach (var component in components)
        {
            var buttons = GetButtonMethods(component.GetType());
            if (buttons.Count > 0)
            {
                buttonTargets.Add(new ButtonTarget(component, buttons));
            }
        }

        RebuildButtonView();
    }

    private void RebuildButtonView()
    {
        if (m_ButtonScrollView == null)
        {
            return;
        }

        m_ButtonScrollView.Clear();

        var groups = buttonTargets
            .GroupBy(target => GetGroupType(target.Component.GetType()))
            .OrderBy(group => group.Key.Name);

        foreach (var group in groups)
        {
            var matchingTargets = group
                .Where(target => target.IsValid && target.Buttons.Any(button => MatchesSearch(target, button)))
                .ToList();

            if (matchingTargets.Count == 0)
            {
                continue;
            }

            var foldout = new Foldout
            {
                text = GetTypeName(group.Key),
                value = GetFoldoutState(group.Key)
            };
            foldout.AddToClassList("button-group");
            foldout.RegisterValueChangedCallback(change =>
            {
                groupFoldouts[group.Key] = change.newValue;
            });

            foreach (var target in matchingTargets)
            {
                AddTargetRows(foldout, target);
            }

            m_ButtonScrollView.Add(foldout);
        }
    }

    private void AddTargetRows(VisualElement parent, ButtonTarget target)
    {
        var matchingButtons = target.Buttons
            .Where(button => MatchesSearch(target, button))
            .ToList();

        foreach (var button in matchingButtons)
        {
            var row = m_ButtonItemAsset.Instantiate();
            var actionButton = row.Q<Button>("action-button");
            var selectButton = row.Q<Button>("select-button");
            var methodLabel = row.Q<Label>("method-name");
            var targetLabel = row.Q<Label>("target-name");

            actionButton.clicked += () => Invoke(button.Method, target.Component);
            actionButton.SetEnabled(target.IsValid && target.IsActive && button.IsEnabled);
            methodLabel.text = button.DisplayName;
            targetLabel.text = target.DisplayName;
            selectButton.tooltip = target.HierarchyPath;
            selectButton.clicked += () => SelectTarget(target);
            selectButton.style.backgroundImage = EditorGUIUtility.IconContent(
                "d_UnityEditor.SceneView").image as Texture2D;

            parent.Add(row);
        }

        if (target.IsValid && !target.IsActive)
        {
            var inactiveLabel = new Label("Inactive or disabled");
            inactiveLabel.AddToClassList("inactive-label");
            parent.Add(inactiveLabel);
        }
    }

    private bool GetFoldoutState(Type type)
    {
        if (!groupFoldouts.TryGetValue(type, out var isExpanded))
        {
            isExpanded = true;
            groupFoldouts[type] = isExpanded;
        }

        return isExpanded;
    }

    private static void SelectTarget(ButtonTarget target)
    {
        if (!target.IsValid)
        {
            return;
        }

        Selection.activeObject = target.Component.gameObject;
        EditorGUIUtility.PingObject(target.Component.gameObject);
    }

    private static List<ButtonMethod> GetButtonMethods(Type componentType)
    {
        return componentType
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(method => new
            {
                Method = method,
                Attribute = method.GetCustomAttribute<DarkButtonAttribute>()
            })
            .Where(item => item.Attribute != null && item.Method.GetParameters().Length == 0)
            .Select(item => new ButtonMethod(
                item.Method,
                item.Attribute,
                string.IsNullOrEmpty(item.Attribute.ButtonName)
                    ? ObjectNames.NicifyVariableName(item.Method.Name)
                    : item.Attribute.ButtonName))
            .ToList();
    }

    private static Type GetGroupType(Type type)
    {
        if (type == null)
        {
            return typeof(MonoBehaviour);
        }

        var currentType = type;
        var parentType = type.BaseType;

        while (parentType != null && parentType != typeof(object))
        {
            if (parentType == typeof(MonoBehaviour))
            {
                return currentType;
            }

            currentType = parentType;
            parentType = parentType.BaseType;
        }

        return type;
    }

    private static string GetTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var name = type.Name;
        var genericNameEnd = name.IndexOf('`');
        var genericTypeName = genericNameEnd >= 0
            ? name.Substring(0, genericNameEnd)
            : name;
        var genericArguments = type
            .GetGenericArguments()
            .Select(GetTypeName);

        return $"{genericTypeName}<{string.Join(", ", genericArguments)}>";
    }

    private bool MatchesSearch(ButtonTarget target, ButtonMethod button)
    {
        var searchText = m_SearchField?.value;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return true;
        }

        return target.DisplayName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
            || button.DisplayName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static void Invoke(MethodInfo method, MonoBehaviour component)
    {
        if (component == null)
        {
            return;
        }

        try
        {
            method.Invoke(component, null);
        }
        catch (TargetInvocationException exception)
        {
            Debug.LogException(exception.InnerException ?? exception, component);
        }
    }

    private sealed class ButtonTarget
    {
        public ButtonTarget(MonoBehaviour component, List<ButtonMethod> buttons)
        {
            Component = component;
            Buttons = buttons;
            DisplayName = component.gameObject.name;
            HierarchyPath = $"{component.gameObject.scene.name} / {component.transform.GetHierarchyPath()}";
        }

        public MonoBehaviour Component { get; }
        public List<ButtonMethod> Buttons { get; }
        public string DisplayName { get; }
        public string HierarchyPath { get; }
        public bool IsValid => Component != null && Component.gameObject != null && Component.transform != null;
        public bool IsActive => Component.isActiveAndEnabled;
    }

    private sealed class ButtonMethod
    {
        public ButtonMethod(MethodInfo method, DarkButtonAttribute attribute, string displayName)
        {
            Method = method;
            Attribute = attribute;
            DisplayName = displayName;
        }

        public MethodInfo Method { get; }
        public DarkButtonAttribute Attribute { get; }
        public string DisplayName { get; }
        public bool IsEnabled => Attribute.Mode == DarkButtonMode.AlwaysEnabled
            || (EditorApplication.isPlaying && Attribute.Mode == DarkButtonMode.EnabledInPlayMode)
            || (!EditorApplication.isPlaying && Attribute.Mode == DarkButtonMode.DisabledInPlayMode);
    }
}

internal static class TransformExtensions
{
    public static string GetHierarchyPath(this Transform transform)
    {
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = $"{transform.name}/{path}";
        }

        return path;
    }
}
