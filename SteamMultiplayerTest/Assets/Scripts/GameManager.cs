using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

public class GameManager : MonoBehaviour
{
    [Header("Dependencies")]
    private NetworkManager _networkManager;

    [Header("Prefabs")] 
    [SerializeField] private PlayerController playerCharacterPrefab;

    // Private variables
    private Dictionary<ulong, ClientPlayer> _playersOwners;
    private List<ClientPlayer> _players;

    #region Unity Methods

    private void Awake()
    {
        // Get dependencies
        _networkManager = FindObjectOfType<NetworkManager>();
        
        // Get player owner dictionary and player list
        _playersOwners = FindObjectsOfType<ClientPlayer>()
            .ToDictionary(clientPlayer => clientPlayer.OwnerClientId, x => x);
        _players = _playersOwners.Select(x => x.Value).ToList();
        
        // Game initialization logic
        InitializeGame();
    }

    #endregion
    
    #region Private Methods

    /// <summary>
    /// Sets up player characters and starts the game
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log("Initializing the game...");
        
        Debug.Log($"Spawning {playerCharacterPrefab.GetType()}s for players...");
        SpawnPlayerCharacters();
    }

    /// <summary>
    /// Spawns <see cref="playerCharacterPrefab"/> prefab for every player present in <see cref="_networkManager"/>
    /// </summary>
    private void SpawnPlayerCharacters()
    {
        var clientPlayers = _networkManager.ConnectedClients;
        
        foreach (var playerClient in clientPlayers)
        {
            var player = playerClient.Value;
            var clientId = playerClient.Key;
            
            var playerController = Instantiate(playerCharacterPrefab);
            
            playerController.NetworkObject.SpawnWithOwnership(clientId);
        }
    }

    #endregion
}