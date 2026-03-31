using UnityEngine;

namespace Framework
{
    public class FrameworkProjectConfig : ScriptableObject
    {
        [field: SerializeField] 
        public string DiscordWebhookUrl1 { get; private set; }
        
        [field: SerializeField] 
        public string DiscordWebhookUrl2 { get; private set; }
        
        [field: SerializeField] 
        public string DiscordWebhookUrl3 { get; private set; }
    }
}