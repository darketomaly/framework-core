using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.UI
{
    public class UIThemesManager : MonoBehaviour
    {
        [SerializeField]
        private ThemeStyleSheet[] m_Themes;

        [SerializeField] 
        private PanelSettings[] m_PanelSettings;
    
        [ContextMenu("Select theme")]
        private void SelectTheme()
        {
            SelectTheme(m_Themes[0]);
        }

        private void SelectTheme(ThemeStyleSheet targetTheme)
        {
            foreach (var panelSetting in m_PanelSettings)
            {
                panelSetting.themeStyleSheet = targetTheme;
            }
        }
    }
}
