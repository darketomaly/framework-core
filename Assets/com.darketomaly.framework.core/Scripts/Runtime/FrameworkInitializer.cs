using UnityEngine;

namespace Framework
{
    public static class FrameworkInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            var projectConfig = Resources.Load<FrameworkProjectConfig>("Framework/Framework project config");
            var prefab = projectConfig.GameManagerPrefab;

            var spawn = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(spawn);
        }
    }
}