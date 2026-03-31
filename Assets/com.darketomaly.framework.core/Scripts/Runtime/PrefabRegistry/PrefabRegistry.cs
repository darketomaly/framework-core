using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#endif

using UnityEngine;

namespace Framework
{
    #if UNITY_EDITOR

    internal class MyCustomBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            
        }
    }
    
    #endif
    
    [CreateAssetMenu]
    public class PrefabRegistry : ScriptableObject
    {
        [System.Serializable]
        public class PrefabRegistryItem
        {
            public string m_Type;
            public GameObject m_Prefab;
        }

        [SerializeField] private List<PrefabRegistryItem> m_Prefabs = new();

        public void Register(System.Type type, MonoBehaviour prefab)
        {
            Debug.Log($"Registering {type} for {prefab.gameObject.name}");

            foreach (PrefabRegistryItem prefabItem in m_Prefabs)
            {
                if (prefabItem.m_Type == type.ToString())
                {
                    prefabItem.m_Prefab = prefab.gameObject;
                    return;
                }
            }

            m_Prefabs.Add(new PrefabRegistryItem() { m_Type = type.ToString(), m_Prefab = prefab.gameObject });
            
            #if UNITY_EDITOR
            
            EditorUtility.SetDirty(Core.PrefabRegistryInstance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            #endif
        }

        public static bool LoadUniquePrefab<T>(out T loadedObject) where T : MonoBehaviour
        {
            foreach (var prefabItem in Core.PrefabRegistryInstance.m_Prefabs)
            {
                if (prefabItem.m_Type == typeof(T).ToString())
                {
                    loadedObject = prefabItem.m_Prefab.GetComponent<T>();
                    Debug.Log("Returning loaded object");
                    return loadedObject;
                }
            }

            Debug.LogError($"Returning null <b>{typeof(T)}</b> prefab.");

            loadedObject = null;
            return false;
        }
    }
}