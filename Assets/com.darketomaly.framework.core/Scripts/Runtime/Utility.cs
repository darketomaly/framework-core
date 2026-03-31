using System.Collections;
using UnityEngine;

namespace Framework
{
    public static partial class Core
    {
        public static class Utility
        {
            public static IEnumerator WaitFrames(int frames)
            {
                for (int i = 0; i < frames; i++) yield return null;
            }

            public static bool LoadUniquePrefab<T>(out T prefab) where T : MonoBehaviour
            {
                #if UNITY_EDITOR

                return EditorUtility.LoadUniquePrefab(out prefab);

                #else
                
                return PrefabRegistry.LoadUniquePrefab(out prefab);

                #endif
            }
        }
    }
}