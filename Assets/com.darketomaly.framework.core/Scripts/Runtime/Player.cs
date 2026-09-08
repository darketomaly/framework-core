using Framework.Events;
using UnityEngine;

namespace Framework
{
    public abstract class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        protected virtual void OnEnable()
        {
            FrameworkEventsFactory.RaiseWithPayload(PlayerEvents.Enabled, this);
        }

        protected virtual void OnDisable()
        {
            FrameworkEventsFactory.Unraise(PlayerEvents.Enabled);
        }

        public virtual void FaceForward()
        {
            // Let children handle this
            // So VR for example handle its own logic
        }
    }
}