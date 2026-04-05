using System;
using UnityEngine;

namespace Framework
{
    public partial class FrameworkProjectConfig : ScriptableObject
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
    }
}