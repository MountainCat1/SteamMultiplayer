using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class HostManager : MonoBehaviour
    {
        #region Events
        
        /// <summary>
        /// Event invoked whenever player joined a lobby and <see cref="Player"/> was instantiated for them
        /// </summary>
        public event Action<Player> OnPlayerJoined;

        /// <summary>
        /// Event invoked whenever player joined a lobby and <see cref="Player"/> was instantiated for them
        /// </summary>
        public event Action<Player> OnPlayerLeft;

        #endregion

        [Header("Dependencies")] [SerializeField]
        private NetworkManager networkManager;

        [SerializeField] private SteamLobbyManager lobbyManager;

        private Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();

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
            var playerObject = connectedClients[clientId].PlayerObject.GetComponent<Player>();

            playerObject.CSteamId.Value = playerCSteamId;
            playerObject.Name.Value = lobbyManager.GetPlayerName(playerObject.CSteamId.Value);
            playerObject.gameObject.name = lobbyManager.GetPlayerName(playerObject.CSteamId.Value);

            _players.Add(clientId, playerObject);

            OnPlayerJoined?.Invoke(playerObject);
        }

        #endregion

        #region Public Methods

        public void StartLobby()
        {
            lobbyManager.HostLobby();
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
            if (networkManager.IsHost)
                return;
            // Clients logic

            Debug.Log("Starting a client...");
            networkManager.StartClient();
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"Player ({clientId} disconnected)");

            var player = _players[clientId];

            OnPlayerLeft?.Invoke(player);

            _players.Remove(clientId);
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            StartCoroutine(HandlePlayerEnteredCoroutine(clientId));
        }

        #endregion
    }
}