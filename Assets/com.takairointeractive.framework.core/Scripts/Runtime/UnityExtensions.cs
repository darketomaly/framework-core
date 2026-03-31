using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public static partial class Core
    {
        private static readonly Dictionary<(Component, object), CachedObject> GetComponentCache = new();
        
        struct CachedObject
        {
            public CachedObject(object obj, bool hadSomethingAtSomePoint)
            {
                m_Object = obj;
                m_HadSomethingAtSomePoint = hadSomethingAtSomePoint;
            }
            
            public object m_Object;
            public bool m_HadSomethingAtSomePoint;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void OnLoad()
        {
            Debug.Log("GetComponent Cache loaded");
            FlushCache();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log("Scene Loaded, flushing cache.");
            FlushCache();
        }

        public static void FlushCache()
        {
            GetComponentCache.Clear();
            Debug.Log("Flushed cache");
        }

        public static T GetCachedComponent<T>(this GameObject keyObject) where T : class
        {
            return keyObject.transform.GetCachedComponent<T>();
        }
        
        public static T GetCachedComponent<T>(this Component keyObject) where T : class
        {
            if (Application.isPlaying)
            {
                if (GetComponentCache.ContainsKey((keyObject, typeof(T))))
                {
                    var cachedObj = GetComponentCache[(keyObject, typeof(T))];
                
                    if (cachedObj.m_Object != null)
                    {
                        return cachedObj.m_Object as T;
                    }
                    else
                    {
                        //If it had something, the object got destroyed, re-cache it
                        if (cachedObj.m_HadSomethingAtSomePoint)
                        {
                            GetComponentCache.Remove((keyObject, typeof(T)));
                        }
                        else
                        {
                            return (T)cachedObj.m_Object;
                        }
                    }
                }
                
                var newObj = keyObject.GetComponent<T>();
                GetComponentCache.Add((keyObject, typeof(T)), new CachedObject(newObj, newObj != null));
                    
                return newObj;
            }
            
            return keyObject.GetComponent<T>();
        }

        public static string GetReadableValue(this Enum source)
        {
            return Regex.Replace(source.ToString(), "(?<!^)([A-Z])", " $1");
        }
    }
}