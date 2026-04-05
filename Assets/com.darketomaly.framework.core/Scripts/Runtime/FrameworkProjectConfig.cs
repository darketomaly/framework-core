using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public class FrameworkProjectConfig : ScriptableObject
    {
        public static FrameworkProjectConfig Instance => Resources.Load<FrameworkProjectConfig>("Framework/Framework project config");

        [field: SerializeField] 
        public string DiscordWebhookUrl1 { get; private set; }
        
        [field: SerializeField] 
        public string DiscordWebhookUrl2 { get; private set; }
        
        [field: SerializeField] 
        public string DiscordWebhookUrl3 { get; private set; }

        [field: SerializeField, Space] 
        public string IpCheckUrl { get; private set; } = "https://api.ipify.org";

        public string DisplayName { get; private set; }
        
        public string PlatformUserId { get; private set; }
        
        public string Platform { get; private set; }
        
        public string Version { get; private set; }

        /// <summary>
        /// Sets the data to be used on Discord webhooks
        /// </summary>
        public void SetDiscordStreamData(string displayName, string platformUserId, string platform, string version)
        {
            DisplayName = displayName;
            PlatformUserId = platformUserId;
            Platform = platform;
            Version = version;
        }
        
        // === Module system for optional packages ===
        [SerializeReference]
        private List<object> m_Modules = new();
        
        public T GetModule<T>() where T : FrameworkModuleData
        {
            return m_Modules.OfType<T>().FirstOrDefault();
        }
        
        public void AddModule<T>(object module) where T : FrameworkModuleData
        {
            if (module == null || m_Modules.Any(m => m.GetType() == module.GetType()))
                return;
            
            m_Modules.Add(module);
        }
    }
    
    public abstract class FrameworkModuleData { }
}