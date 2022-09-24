using System;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UILobbyScreen : MonoBehaviour
    {
        // UI Elements
        [SerializeField] private TextMeshProUGUI lobbyTitle;
        
        // Injection
        [SerializeField] private SteamLobbyManager steamLobbyManager;
        [SerializeField] private NetworkManager networkManager;

        private void Awake()
        {
            steamLobbyManager.OnLobbyHosted += OnLobbyHosted;
            steamLobbyManager.OnLobbyEntered += OnLobbyEntered;
            steamLobbyManager.OnLobbyDataUpdated += OnLobbyDataUpdated;
            
            networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        }

        private void OnLobbyDataUpdated()
        {
            lobbyTitle.text = steamLobbyManager.LobbyName;
        }

        private void OnClientConnectedCallback(ulong obj)
        {
            throw new NotImplementedException();
        }
        
        #region Event Handlers
        
        private void OnLobbyHosted()
        {
            
        }
        
        private void OnLobbyEntered()
        {
            
        }
        
        #endregion
    }
}
