using System;
using System.Collections.Generic;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI
{
    [RequireComponent(typeof(UIMenuScreen))]
    public class UILobbyScreen : MonoBehaviour
    {
        // UI Elements
        [Header("UI Elements References")]
        [SerializeField] private TextMeshProUGUI lobbyTitle;
        [SerializeField] private Transform playerEntriesContainer;

        [Header("Prefabs")] 
        [SerializeField] private UIPlayerEntry playerEntryPrefab;
        
        // Injection
        [Header("Dependencies")]
        [SerializeField] private SteamLobbyManager steamLobbyManager;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private HostManager hostManager;
        private UIMenuScreen _menuScreen;

        // Variables
        private Dictionary<Player, UIPlayerEntry> _playerEntries = new Dictionary<Player, UIPlayerEntry>();

        #region Unity Methods

        private void Awake()
        {
            steamLobbyManager.OnSteamLobbyDataUpdated += OnSteamLobbyDataUpdated;
            
            hostManager.OnPlayerJoined += OnPlayerJoined;
            hostManager.OnPlayerLeft += OnPlayerLeft;
            hostManager.OnLobbyStart += OnLobbyStart;

            _menuScreen = GetComponent<UIMenuScreen>();
            
            _menuScreen.OnScreenHide += OnScreenHide;
        }

        #endregion

        #region Public Methods

        public void StartGame()
        {
            hostManager.StartGame();
        }

        #endregion
        
        #region Private Methods

        private void ClearEntries()
        {
            foreach (var entry in _playerEntries.Values)
            {
                Destroy(entry.gameObject);
            }
            
            _playerEntries.Clear();
        }

        #endregion

        #region Event Handlers

        private void OnLobbyStart()
        {
            _menuScreen.Show();
        }
        
        private void OnPlayerLeft(Player player)
        {
            var entry = _playerEntries[player];
            
            Destroy(entry.gameObject);
            
            _playerEntries.Remove(player);
        }

        private void OnPlayerJoined(Player player)
        {
            var playerEntry = Instantiate(playerEntryPrefab, playerEntriesContainer);
            
            playerEntry.Initialize(player);
            
            _playerEntries.Add(player, playerEntry);
        }
        
        private void OnScreenHide()
        {
            ClearEntries();
            
            hostManager.LeaveLobby();
        }
        
        private void OnSteamLobbyDataUpdated()
        {
            Debug.Log("Updating lobby data...");
            lobbyTitle.text = steamLobbyManager.LobbyName;
        }
        
        #endregion
    }
}
