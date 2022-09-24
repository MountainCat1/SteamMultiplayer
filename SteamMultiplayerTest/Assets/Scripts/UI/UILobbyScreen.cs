using System;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    [RequireComponent(typeof(UIMenuScreen))]
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

            var menuScreen = GetComponent<UIMenuScreen>();
            
            menuScreen.OnScreenHide += OnScreenHide;
        }

        private void OnScreenHide()
        {
            steamLobbyManager.LeaveLobby();
        }

        private void OnLobbyDataUpdated()
        {
            Debug.Log("Updating lobby data...");
            lobbyTitle.text = steamLobbyManager.LobbyName;
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
