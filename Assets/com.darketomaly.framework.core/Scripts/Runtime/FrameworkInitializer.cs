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

            if (prefab)
            {
                var spawn = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(spawn);
            } else
            {
                projectConfig.LogError("Game manager prefab not found, please assign one on Resources/Framework project config scriptable.");
            }
        }
    }
}