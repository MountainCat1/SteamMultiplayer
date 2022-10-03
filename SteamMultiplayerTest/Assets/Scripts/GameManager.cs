using System;
using Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private HostManager hostManager;
    [SerializeField] private NetworkManager networkManager;

    [Header("Prefabs")] 
    [SerializeField] private PlayerController playerCharacterPrefab;


    #region Unity Methods

    private void Awake()
    {
           
    }

    #endregion
    


    /// <summary>
    /// Sets up player characters and starts the game
    /// </summary>
    public void StartGame()
    {
        Debug.Log("Staring the game...");
        
        Debug.Log($"Spawning {playerCharacterPrefab.GetType()}s for players...");
        SpawnPlayerCharacters();
    }

    /// <summary>
    /// Spawns <see cref="playerCharacterPrefab"/> prefab for every player present in <see cref="hostManager"/>
    /// </summary>
    private void SpawnPlayerCharacters()
    {
        var clientPlayers = hostManager.Players;
        foreach (var playerClient in clientPlayers)
        {
            var player = playerClient.Value;
            var clientId = playerClient.Key;
            
            var playerController = Instantiate(playerCharacterPrefab);
            
            playerController.NetworkObject.SpawnWithOwnership(clientId);
        }
    }
}