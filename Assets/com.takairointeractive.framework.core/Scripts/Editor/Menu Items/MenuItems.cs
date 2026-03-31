using System.IO;
using UnityEditor;

namespace Framework.EditorTools
{
    public class MenuItems 
    {
        [MenuItem("Assets/Replace by")]
        private static void DoReplaceBy()
        {
            var selected = Selection.activeObject;
            if (selected == null) return;

            var path = AssetDatabase.GetAssetPath(selected);
            var extension = Path.GetExtension(path);
        
            var replacePath = EditorUtility.OpenFilePanel("Replace by", "", extension.Replace(".", ""));
            if (string.IsNullOrEmpty(replacePath)) return;

            var replaceBytes = File.ReadAllBytes(replacePath);
        
            File.WriteAllBytes(path, replaceBytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        [MenuItem("Assets/Replace by", true)]
        private static bool ReplaceByValidation()
        {
            return Selection.activeObject != null;
        }
    }
}