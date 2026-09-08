using UnityEngine;

namespace Framework
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance;
    
        private void Awake()
        {
            Instance = (T)this;
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        
        }
    }
}