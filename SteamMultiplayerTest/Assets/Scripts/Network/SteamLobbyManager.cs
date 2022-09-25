using System;
using System.Collections;
using System.Text;
using Netcode.Transports;
using Steamworks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class SteamLobbyManager : MonoBehaviour
    {
        // Public events

        /// <summary>
        /// Event invoked whenever local instance hosts lobby
        /// </summary>>
        public event Action OnLobbyHosted;
        /// <summary>
        /// Event invoked whenever local instance joins a lobby
        /// </summary>
        public event Action OnLobbyEntered;
        /// <summary>
        /// Event invoked whenever steam lobby data is retrieved
        /// </summary>
        public event Action OnLobbyDataUpdated;

        // Steam Callbacks
        protected Callback<LobbyEnter_t> LobbyEnteredCallback;
        protected Callback<LobbyCreated_t> LobbyCreatedCallback;
        protected Callback<GameLobbyJoinRequested_t> JoinRequestCallback;
        protected Callback<LobbyDataUpdate_t> LobbyDataUpdatedCallback;

        // Variables
        public ulong CurrentLobbyID { get; set; }
        private CSteamID CurrentLobbySteamID { get; set; }
        
        protected const string HostAddressKey = "HostAddress";
        protected const string LobbyNameKey = "name";
    
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private SteamNetworkingTransport networkTransport;

        // Configuration
        [SerializeField] private int maxPlayersInLobby = 4;
        [SerializeField] private int steamCallbacksPerSecond = 30;

        #region Unity Methods
        
        private void OnEnable()
        {
            Debug.Log("Starting steam...");
            SteamAPI.Init();
        }
        
        private void OnDisable()
        {
            Debug.Log("Shutting down steam...");
            SteamAPI.Shutdown();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnDisable();
            }
        }

        private void Start()
        {
            if (!SteamAPI.IsSteamRunning())
            {
                Debug.LogError("Steam is not initialized");
                return;
            }

            // Register steam callbacks
            LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreatedSteamHandler);
            LobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEnteredSteamHandler);
        
            JoinRequestCallback = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequestSteamHandler);
            
            LobbyDataUpdatedCallback = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdatedSteamHandler);

            StartCoroutine(SteamCallbackCoroutine());
        }

        #endregion

        #region Lobby Properties

        public string LobbyName => SteamMatchmaking.GetLobbyData(CurrentLobbySteamID, LobbyNameKey);

        #endregion
        
        #region Public Methods

        public void RunCallbacks()
        {
            SteamAPI.RunCallbacks();
        }
        
        public void HostLobby()
        {
            Debug.Log("Creating Steam lobby...");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayersInLobby);
        }
        
        public void JoinLobby(CSteamID lobbyId)
        {
            SteamMatchmaking.JoinLobby(lobbyId);
        }

        public void RequestLobbyData(CSteamID lobbyId)
        {
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }

        public void LeaveLobby()
        {
            Debug.Log("Leaving lobby");
            networkManager.Shutdown();
            SteamMatchmaking.LeaveLobby(CurrentLobbySteamID);
        }

        public CSteamID GetPlayerSteamId(int playerIndex)
        {
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(CurrentLobbySteamID);
            return SteamMatchmaking.GetLobbyMemberByIndex(CurrentLobbySteamID, playerIndex);
        }
        
        public string GetPlayerName(ulong playerCSteamId)
        {
            return SteamFriends.GetFriendPersonaName((CSteamID)playerCSteamId);
        }

        #endregion

        #region SteamCallbackHandlers

        private void OnLobbyDataUpdatedSteamHandler(LobbyDataUpdate_t lobbyData)
        {
            OnLobbyDataUpdated?.Invoke();
        }
        
        private void OnLobbyCreatedSteamHandler(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError($"Failed to create a lobby: {callback.m_eResult}");
                return;
            }

            Debug.Log($"Lobby created successfully ({callback.m_ulSteamIDLobby})");

            Debug.Log("Creating host...");
            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey,
                SteamUser.GetSteamID().ToString());

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                LobbyNameKey,
                $"{SteamFriends.GetPersonaName()}'s lobby");
        }

        private void OnJoinRequestSteamHandler(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request to join lobby");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEnteredSteamHandler(LobbyEnter_t callback)
        {
            Debug.Log("Entered steam lobby");
            var lobbyId = new CSteamID(callback.m_ulSteamIDLobby);

            // Everyone
            CurrentLobbyID = callback.m_ulSteamIDLobby;
            CurrentLobbySteamID = new CSteamID(CurrentLobbyID);
            
            // Host
            if (networkManager.IsHost)
            {
                OnLobbyHosted?.Invoke();
                OnLobbyEntered?.Invoke();
                return;
            }
            
            // Clients
            networkTransport.ConnectToSteamID = callback.m_ulSteamIDLobby;

            networkManager.StartClient();
            
            OnLobbyEntered?.Invoke();
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Coroutine running all Steam API callbacks
        /// </summary>
        /// <returns></returns>
        private IEnumerator SteamCallbackCoroutine()
        {
            float delay = 1f / steamCallbacksPerSecond;

            while (SteamAPI.IsSteamRunning())
            {
                yield return new WaitForSeconds(delay);
            
                SteamAPI.RunCallbacks();
            }
        }

        #endregion
        
        #region Private Methods

        public void SendSteamLobbyMessage(string message)
        {
            // https://partner.steamgames.com/doc/api/ISteamMatchmaking#SendLobbyChatMsg
            SteamMatchmaking.SendLobbyChatMsg(CurrentLobbySteamID, Encoding.UTF8.GetBytes(message), message.Length + 1);
        }

        #endregion
    }
}