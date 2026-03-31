using UnityEditor;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class FrameworkAutoOpen 
    {
        static FrameworkAutoOpen()
        {
            bool hasSetup = EditorPrefs.GetBool("FrameworkSetup", false);

            if (!hasSetup)
            {
                EditorApplication.delayCall += OpenWindow;
            }
        }
    
        private static void OpenWindow()
        {
            EditorApplication.ExecuteMenuItem("Tools/Framework/Setup");
            EditorPrefs.SetBool("FrameworkSetup", true);
        }
    }
}