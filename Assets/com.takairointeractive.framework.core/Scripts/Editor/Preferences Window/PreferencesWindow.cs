using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Editor
{
    public class PreferencesWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Tools/Framework/Preferences")]
        public static void ShowExample()
        {
            PreferencesWindow wnd = GetWindow<PreferencesWindow>();
            wnd.titleContent = new GUIContent("Framework Preferences");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            DrawDefaultValues();
            Subscribe();
        }

        private void DrawDefaultValues()
        {
            var root = rootVisualElement;

            root.Q<ColorField>("LogColor").SetValueWithoutNotify(Core.Prefs.GetColor(Core.Prefs.Key.LogColor));
            root.Q<ColorField>("ImportantLogColor").SetValueWithoutNotify(Core.Prefs.GetColor(Core.Prefs.Key.ImportantLogColor));
            root.Q<ColorField>("WarningLogColor").SetValueWithoutNotify(Core.Prefs.GetColor(Core.Prefs.Key.WarningLogColor));
            root.Q<ColorField>("ErrorLogColor").SetValueWithoutNotify(Core.Prefs.GetColor(Core.Prefs.Key.ErrorLogColor));
        }
        
        private void Subscribe()
        {
            VisualElement root = rootVisualElement;
            
            root.Q<ColorField>("LogColor").RegisterValueChangedCallback(changeEvent => { OnColorValueChanged(changeEvent, Core.Prefs.Key.LogColor); });
            root.Q<ColorField>("ImportantLogColor").RegisterValueChangedCallback(changeEvent => { OnColorValueChanged(changeEvent, Core.Prefs.Key.ImportantLogColor); });
            root.Q<ColorField>("WarningLogColor").RegisterValueChangedCallback(changeEvent => { OnColorValueChanged(changeEvent, Core.Prefs.Key.WarningLogColor); });
            root.Q<ColorField>("ErrorLogColor").RegisterValueChangedCallback(changeEvent => { OnColorValueChanged(changeEvent, Core.Prefs.Key.ErrorLogColor); });

            root.Q<Toggle>("FullTypePath").RegisterValueChangedCallback(changeEvent => { OnToggleValueChanged(changeEvent, Core.Prefs.Key.FullTypePath); });
        }

        private void OnToggleValueChanged(ChangeEvent<bool> evt, string key)
        {
            EditorPrefs.SetBool(key, evt.newValue);
        }
        
        private void OnEnumFlagsValueChanged(ChangeEvent<Enum> evt, string key)
        {
            EditorPrefs.SetInt(key, Convert.ToInt32(evt.newValue));
        }

        private void OnColorValueChanged(ChangeEvent<Color> evt, string key)
        {
            var htmlStringRgba = ColorUtility.ToHtmlStringRGB(evt.newValue);
            EditorPrefs.SetString(key, htmlStringRgba);
        }
    }
}
