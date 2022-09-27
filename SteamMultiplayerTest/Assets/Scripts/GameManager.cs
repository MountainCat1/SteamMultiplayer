using Network;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private HostManager _hostManager;
    [SerializeField] private NetworkManager _networkManager;

    [Header("Prefabs")] 
    [SerializeField] private PlayerController _playerCharacterPrefab;
    
    public void StartGame()
    {
        Debug.Log("Staring the game...");
        
        Debug.Log($"Spawning {_playerCharacterPrefab.GetType()}s for players...");
        SpawnPlayerCharacters();
    }

    /// <summary>
    /// Spawns <see cref="_playerCharacterPrefab"/> prefab for every player present in <see cref="_hostManager"/>
    /// </summary>
    private void SpawnPlayerCharacters()
    {
        var clientPlayers = _hostManager.Players;
        foreach (var playerClient in clientPlayers)
        {
            var player = playerClient.Value;
            var clientId = playerClient.Key;
            
            var playerController = Instantiate(_playerCharacterPrefab);
            
            playerController.NetworkObject.SpawnWithOwnership(clientId);
        }
    }
}