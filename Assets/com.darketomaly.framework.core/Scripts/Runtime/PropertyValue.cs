using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [System.Serializable]
    public class PropertyValue<T>
    {
        public Action<T> ValueChanged;
        
        [SerializeField]
        private T m_value;

        public T Value
        {
            get => m_value;
                
            set
            {
                if (!EqualityComparer<T>.Default.Equals(m_value, value))
                {
                    m_value = value;
                    ValueChanged?.Invoke(m_value);
                }
            }
        }
        
        public PropertyValue() { }

        public PropertyValue(T defaultValue)
        {
            m_value = defaultValue;
        }

        public void SubscribeAndExecute(Action<T> action)
        {
            ValueChanged += action;
            action?.Invoke(Value);
        }

        public void Unsubscribe(Action<T> action)
        {
            ValueChanged -= action;
        }
    }
}