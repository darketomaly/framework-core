using UnityEditor;

namespace Framework
{
    public abstract class ButtonEditorBase : UnityEditor.Editor
    {
        private readonly ButtonDrawer m_drawer = new ButtonDrawer();

        protected virtual void OnEnable()
        {
            if (target != null) m_drawer.Init(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            m_drawer.Draw(targets);
        }
    }
}