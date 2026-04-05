using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ErrorHandler : MonoBehaviour
    {
        private static ErrorHandler Instance;
        
        private Dictionary<string, QueuedAction> m_queuedActions = new();

        private class QueuedAction
        {
            public Action<Action> Action;
            public int Reattempts;
            public Action OnFailure;
            public Coroutine ActiveCoroutine;
        }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Automatically re-attempts an action in case of failure.
        /// </summary>
        /// <param name="uniqueContext">Unique. If this is called while it's running, it will stop any re-attempt loops and start again.</param>
        /// <param name="action">Method to execute. Provides an onFailure action that you need to invoke in order to re-attempt.</param>
        /// <param name="maxAttempts">Maximum amount of re-attempts.</param>
        public static void Execute(string uniqueContext, Action<Action> action, int maxAttempts = 10)
        {
            if (Instance.m_queuedActions.ContainsKey(uniqueContext))
            {
                if (Instance.m_queuedActions[uniqueContext].ActiveCoroutine != null)
                {
                    Instance.LogError($"<color=olive>{uniqueContext}</color> was already running, stopping and re-starting it.");
                    Instance.StopCoroutine(Instance.m_queuedActions[uniqueContext].ActiveCoroutine);
                }
                else
                {
                    Instance.Log($"Clearing old <color=olive>{uniqueContext}</color> cache.");
                }

                Instance.m_queuedActions.Remove(uniqueContext);
            }
            
            var queuedAction = new QueuedAction();
            queuedAction.Action = action;
            Instance.m_queuedActions.Add(uniqueContext, queuedAction);
            
            queuedAction.OnFailure += delegate
            {
                queuedAction.Reattempts++;
                
                if (queuedAction.Reattempts > maxAttempts)
                {
                    Instance.LogError($"Maximum number of re-attempts reached, not re-attempting <color=olive>{uniqueContext}</color> anymore.");
                    queuedAction.ActiveCoroutine = null;
                }
                else
                {
                    Instance.Log($"Failed to do <color=olive>{uniqueContext}</color>, re-attempting in <color=olive>{queuedAction.Reattempts}</color> second{(queuedAction.Reattempts > 1 ? "s" : "")}");
                    queuedAction.ActiveCoroutine = Instance.StartCoroutine(Instance.Reattempt(queuedAction));
                }
            };
            
            action?.Invoke(queuedAction.OnFailure);
        }
        
        private IEnumerator Reattempt(QueuedAction queuedAction)
        {
            yield return new WaitForSeconds(queuedAction.Reattempts);
            
            queuedAction.ActiveCoroutine = null;
            
            Instance.Log($"Reattempted, number of re-attempts: <color=olive>{queuedAction.Reattempts}</color>");
            queuedAction.Action.Invoke(queuedAction.OnFailure);
        }
    }
}