using System.Collections.Generic;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;

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
        private Dictionary<ClientPlayer, UIPlayerEntry> _playerEntries = new Dictionary<ClientPlayer, UIPlayerEntry>();

        #region Unity Methods

        private void Awake()
        {
            steamLobbyManager.OnSteamLobbyDataUpdated += OnSteamLobbyDataUpdated;
            
            hostManager.OnPlayerJoined += OnPlayerJoined;
            hostManager.OnPlayerLeft += OnPlayerLeft;
            hostManager.OnLobbyLocalJoin += OnLobbyLocalJoin;

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

        private void OnLobbyLocalJoin()
        {
            _menuScreen.Show();
        }
        
        private void OnPlayerLeft(ClientPlayer clientPlayer)
        {
            var entry = _playerEntries[clientPlayer];
            
            Destroy(entry.gameObject);
            
            _playerEntries.Remove(clientPlayer);
        }

        private void OnPlayerJoined(ClientPlayer clientPlayer)
        {
            var playerEntry = Instantiate(playerEntryPrefab, playerEntriesContainer);
            
            playerEntry.Initialize(clientPlayer);
            
            _playerEntries.Add(clientPlayer, playerEntry);
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
