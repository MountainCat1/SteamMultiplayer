using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class ServerMessageLogger : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Event invoked whenever server log is received
        /// </summary>
        public event Action<ServerLog> OnServerLogAdded;

        #endregion
        
        
        
        private readonly List<ServerLog> _serverLogs = new List<ServerLog>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            Application.logMessageReceived += OnApplicationLogReceived;
        }

        private void OnApplicationLogReceived(string message, string stacktrace, LogType type)
        {
            if(type != LogType.Log)
                return;
            
            LogRpc(message, DateTime.Now);
        }

        [ClientRpc]
        private void LogRpc(string message, DateTime timeStamp)
        {
            var log = new ServerLog()
            {
                Message = message,
                TimeStamp = timeStamp
            };
            
            _serverLogs.Add(log);
            
            OnServerLogAdded?.Invoke(log);
        }
    }

    public class ServerLog
    {
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}