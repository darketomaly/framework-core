using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Events
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OneShotAttribute : Attribute { }
    
    public class FrameworkEventsFactory
    {
        private static readonly Dictionary<(Type, object), Delegate> EventSubscribers = new();
        private static readonly HashSet<(Type, object)> FiredEvents = new();
        private static readonly Dictionary<(Type, object), object> FiredPayloads = new();
        private static readonly Dictionary<(Type, object), bool> OneShotCache = new();
        
        private static bool IsOneShot<TEvent>(TEvent eventType) where TEvent : Enum
        {
            var key = (typeof(TEvent), (object)eventType);
 
            if (OneShotCache.TryGetValue(key, out var cached))
                return cached;
 
            var field = typeof(TEvent).GetField(eventType.ToString());
            var isOneShot = field.GetCustomAttribute<OneShotAttribute>() != null;
            OneShotCache[key] = isOneShot;
            return isOneShot;
        }
        
        public static void Raise<TEvent>(TEvent eventType) where TEvent : Enum
        {
            var key = (typeof(TEvent), (object)eventType);
 
            if (!IsOneShot(eventType))
                FiredEvents.Add(key);
 
            if (EventSubscribers.TryGetValue(key, out var del))
                (del as Action)?.Invoke();
        }
 
        /// <summary>
        /// Raise an event and send it something. 
        /// </summary>
        /// <param name="eventType">Event enum, you can use anything, your subscribers will use it.</param>
        /// <param name="payload">Thing to send</param>
        public static void RaiseWithPayload<TEvent, T>(TEvent eventType, T payload) where TEvent : Enum
        {
            var key = (typeof(TEvent), (object)eventType);
 
            if (!IsOneShot(eventType))
            {
                FiredEvents.Add(key);
                FiredPayloads[key] = payload;
            }
 
            if (EventSubscribers.TryGetValue(key, out var del))
                (del as Action<T>)?.Invoke(payload);
        }
 
        public static void Unraise<TEvent>(TEvent eventType) where TEvent : Enum
        {
            var key = (typeof(TEvent), (object)eventType);
            FiredEvents.Remove(key);
            FiredPayloads.Remove(key);
        }
 
        public static void Subscribe<TEvent>(TEvent eventType, Action action) where TEvent : Enum
        {
            var key = (typeof(TEvent), (object)eventType);
 
            if (FiredEvents.Contains(key))
                action();
 
            EventSubscribers.TryGetValue(key, out var existing);
            EventSubscribers[key] = Delegate.Combine(existing, action);
        }
 
        public static void Unsubscribe<TEvent>(TEvent eventType, Action action) where TEvent : Enum
        {
            var key = (typeof(TEvent), (object)eventType);
            if (!EventSubscribers.TryGetValue(key, out var existing))
                return;
 
            var result = Delegate.Remove(existing, action);
            if (result == null) EventSubscribers.Remove(key);
            else EventSubscribers[key] = result;
        }
 
        /// <summary>
        /// If an event is raised with payload, you have to listen to it with this method.
        /// </summary>
        public static void SubscribeWithPayload<T>(Enum eventType, Action<T> action)
        {
            var key = (eventType.GetType(), (object)eventType);
 
            if (FiredEvents.Contains(key) && FiredPayloads.TryGetValue(key, out var payload))
                action((T)payload);
 
            EventSubscribers.TryGetValue(key, out var existing);
            EventSubscribers[key] = Delegate.Combine(existing, action);
        }
 
        public static void UnsubscribeWithPayload<T>(Enum eventType, Action<T> action)
        {
            var key = (eventType.GetType(), (object)eventType);

            if (!EventSubscribers.TryGetValue(key, out var existing))
                return;
 
            var result = Delegate.Remove(existing, action);
            if (result == null) EventSubscribers.Remove(key);
            else EventSubscribers[key] = result;
        }
    }
}