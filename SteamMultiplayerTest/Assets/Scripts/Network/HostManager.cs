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
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private SteamLobbyManager lobbyManager;

        private Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();

        #region Unity Region

        private void OnEnable()
        {
            networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }

        private void OnDisable()
        {
            networkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"Player ({clientId} disconnected)");

            _players.Remove(clientId);
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            StartCoroutine(HandlePlayerEnteredCoroutine(clientId));
        }

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
            
            if(!networkManager.IsHost)
                yield break;
            
            // Handle player joining lobby
            var connectedClients = networkManager.ConnectedClients;
            var playerObject = connectedClients[clientId].PlayerObject.GetComponent<Player>();
            
            playerObject.CSteamId.Value = playerCSteamId;
            playerObject.Name.Value = lobbyManager.GetPlayerName(playerObject.CSteamId.Value);
            playerObject.gameObject.name = lobbyManager.GetPlayerName(playerObject.CSteamId.Value);
            
            _players.Add(clientId, playerObject);
        }
        
        #endregion
    }
}