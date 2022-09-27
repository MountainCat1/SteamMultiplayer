using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Network
{
    public class HostManager : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Event invoked whenever local player joins a lobby
        /// </summary>
        public event Action OnLobbyStart; 
        /// <summary>
        /// Event invoked whenever player joined a lobby and <see cref="ClientPlayer"/> was instantiated for them
        /// </summary>
        public event Action<ClientPlayer> OnPlayerJoined;

        /// <summary>
        /// Event invoked whenever player joined a lobby and <see cref="ClientPlayer"/> was instantiated for them
        /// </summary>
        public event Action<ClientPlayer> OnPlayerLeft;

        #endregion

        [Header("Dependencies")] 
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private SteamLobbyManager lobbyManager;
        

        [SerializeField] public SceneReference initialScene;

        public Dictionary<ulong, ClientPlayer> Players { get; } = new();

        #region Unity Methods

        private void Awake()
        {
            networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;

            lobbyManager.OnSteamLobbyCreated += OnSteamLobbyCreated;
            lobbyManager.OnSteamLobbyEntered += OnSteamLobbyEntered;
            lobbyManager.OnSteamLobbyLocalLeft += OnSteamLobbyLocalLeft;
        }
        
        #endregion

        #region Private Methods

        private IEnumerator HandlePlayerEnteredCoroutine(ulong clientId)
        {
            ulong playerCSteamId = 0;
            while (playerCSteamId == 0)
            {
                lobbyManager.RunCallbacks();

                playerCSteamId = (ulong)lobbyManager.GetPlayerSteamId(networkManager.ConnectedClients.Count - 1);

                yield return new WaitForSeconds(0f);
            }

            Debug.Log($"Player (clientId: {clientId}, CSteamId: {playerCSteamId}) connected");

            if (!networkManager.IsHost)
                yield break;

            // Handle player joining lobby
            var connectedClients = networkManager.ConnectedClients;
            var playerObject = connectedClients[clientId].PlayerObject.GetComponent<ClientPlayer>();

            playerObject.CSteamId.Value = playerCSteamId;
            playerObject.Name.Value = lobbyManager.GetPlayerName(playerObject.CSteamId.Value);
            playerObject.gameObject.name = lobbyManager.GetPlayerName(playerObject.CSteamId.Value);

            Players.Add(clientId, playerObject);

            OnPlayerJoined?.Invoke(playerObject);
        }

        #endregion

        #region Public Methods

        public void StartGame()
        {
            networkManager.SceneManager.LoadScene(initialScene.ScenePath, LoadSceneMode.Single);
        }
        
        public void HostLobby()
        {
            lobbyManager.HostLobby();
        }

        public void LeaveLobby()
        {
            lobbyManager.LeaveLobby();
        }
        
        #endregion

        #region Event Handlers

        private void OnSteamLobbyLocalLeft()
        {
            Debug.Log("Shutting down Network Manager...");
            networkManager.Shutdown();
        }
        
        private void OnSteamLobbyCreated()
        {
            Debug.Log("Starting a host...");
            networkManager.StartHost();
        }

        private void OnSteamLobbyEntered()
        {
            OnLobbyStart?.Invoke();
            
            if (networkManager.IsHost)
                return;
            // Clients logic

            Debug.Log("Starting a client...");
            networkManager.StartClient();
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"Player ({clientId} disconnected)");

            var player = Players[clientId];

            OnPlayerLeft?.Invoke(player);

            Players.Remove(clientId);
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            StartCoroutine(HandlePlayerEnteredCoroutine(clientId));
        }

        #endregion
    }
}