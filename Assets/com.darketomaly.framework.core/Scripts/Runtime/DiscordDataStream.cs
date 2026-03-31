using System;
using System.Collections;
using System.Text;
using Framework;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectCastaway.API.Discord
{
    public class DiscordDataStream : MonoBehaviour
    {
        private static DiscordDataStream Instance;

        private string m_lastMessage;
        private float m_lastMessageAt;
        private string m_publicIp;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartCoroutine(GetPublicIP());
        }

        private IEnumerator GetPublicIP()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(FrameworkProjectConfig.Instance.IpCheckUrl))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.LogError($"Failed to get public IP: {www.error}");
                }
                else
                {
                    var publicIP = www.downloadHandler.text.Trim();
                    m_publicIp = publicIP;
                    this.Log($"Public ip: {publicIP}");
                }
            }
        }

        public static void Push(string message)
        {
            if (Instance.m_lastMessage == message)
            {
                if (Time.time - Instance.m_lastMessageAt < 20f)
                {
                    Instance.Log("Sending the same message too fast");
                    return;
                }
            }
            
            Instance.m_lastMessage = message;
            Instance.m_lastMessageAt = Time.time;
            Instance.StartCoroutine(Instance.SendWebhook(message, FrameworkProjectConfig.Instance.DiscordWebhookUrl1));
        }
        
        public static void Push(string message, string webhookUrl)
        {
            if (Instance.m_lastMessage == message)
            {
                if (Time.time - Instance.m_lastMessageAt < 20f)
                {
                    Instance.Log("Sending the same message too fast");
                    return;
                }
            }
            
            Instance.m_lastMessage = message;
            Instance.m_lastMessageAt = Time.time;
            Instance.StartCoroutine(Instance.SendWebhook(message, webhookUrl));
        }

        private IEnumerator SendWebhook(string message, string webhookUrl)
        {
            // User data
            
            var data = new
            {
                FrameworkProjectConfig.Instance.DisplayName,
                FrameworkProjectConfig.Instance.PlatformUserId,
                PublicIP = $"{m_publicIp}",
                FrameworkProjectConfig.Instance.Platform,
                FrameworkProjectConfig.Instance.Version,
            };
            
            // Discord embed

            if (string.IsNullOrEmpty(message))
            {
                message = "...";
            }
            
            var embed = new Embed
            {
                title = $"{data.Platform} {data.Version}",
                description = message[..Math.Min(4096, message.Length)],
                footer = new Footer
                {
                  text  =  $"{data.DisplayName} | {data.PlatformUserId} | {data.PublicIP}",
                  icon_url = "https://picsum.photos/128/128"
                },
                color = 2303786
            };

            var payload = new Payload
            {
                embeds = new[] { embed }
            };
            
            // Request
            
            string json = JsonUtility.ToJson(payload);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(webhookUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                this.Log($"Sending {json}");
                
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    this.LogError(request.error);
                }
            }
        }
    }

    [System.Serializable]
    public class Embed
    {
        public string title;
        public string description;
        public Footer footer;
        public int color;
    }
    
    [System.Serializable]
    public class Payload
    {
        public Embed[] embeds;
    }

    [System.Serializable]
    public class Footer
    {
        public string text;
        public string icon_url;
    }
}