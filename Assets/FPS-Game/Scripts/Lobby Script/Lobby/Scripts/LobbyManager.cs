using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_CHARACTER = "Character";
    // public const string KEY_GAME_MODE = "GameMode";
    public const string KEY_START_GAME = "Start";

    public event EventHandler OnLeftLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    // public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
    public event EventHandler<EventArgs> OnGameStarted;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    // public enum GameMode
    // {
    //     PvE,
    //     PvP
    // }

    // public enum PlayerCharacter
    // {
    //     Marine,
    //     Ninja,
    //     Zombie
    // }

    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    public static Lobby joinedLobby;
    public static string playerName;
    private string joinedLobbyCode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetPlayerName() { return playerName; }

    private void Update()
    {
        HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async void Authenticate(string name)
    {
        playerName = name;

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            // do nothing
            // Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void HandleRefreshLobbyList()
    {
        // Nếu lobby đã được khởi tạo và player đã sign in thì mới có thể refresh lobby list được
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 10f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 25f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        if (joinedLobby == null)
        {
            return; // Exit if joinedLobby is null to avoid NullReferenceException
        }

        lobbyPollTimer -= Time.deltaTime;
        if (lobbyPollTimer < 0f)
        {
            float lobbyPollTimerMax = 1.5f;
            lobbyPollTimer = lobbyPollTimerMax;

            try
            {
                // Attempt to get lobby details
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                    return;
                }

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (joinedLobby != null && joinedLobby.Data[KEY_START_GAME].Value != "0")
                {
                    // Start Game!
                    if (!IsLobbyHost()) // Lobby Host already joined Relay
                    {
                        if (SceneManager.GetActiveScene().name == "Play Scene") return;

                        // GameSceneManager.Instance.LoadNextScene();
                        GameSceneManager.Instance.LoadScene("Play Scene");

                        Relay.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                    }

                    //joinedLobby = null;

                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
                else return;

            }
            catch (LobbyServiceException e)
            {
                if (e.Message.Contains("lobby is private"))
                {
                    Debug.LogWarning("The lobby has been set to private or is no longer accessible. Redirecting to the Lobby List UI.");

                    // Assume the lobby is no longer accessible - handle this by redirecting
                    // GameSceneManager.Instance.LoadPreviousScene();
                    GameSceneManager.Instance.LoadScene("Lobby Room");

                    joinedLobby = null;
                    return;
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.LogError("Attempted to access null lobby: " + ex.Message);
            }
        }
    }

    public Lobby GetJoinedLobby()
    {
        joinedLobbyCode = joinedLobby.LobbyCode;
        return joinedLobby;
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    public static Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            /*{ KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Marine.ToString()) }*/
        });
    }

    // public void ChangeGameMode()
    // {
    //     if (IsLobbyHost())
    //     {
    //         GameMode gameMode =
    //             Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);

    //         switch (gameMode)
    //         {
    //             default:
    //             case GameMode.PvE:
    //                 gameMode = GameMode.PvP;
    //                 break;
    //             case GameMode.PvP:
    //                 gameMode = GameMode.PvE;
    //                 break;
    //         }

    //         UpdateLobbyGameMode(gameMode);
    //     }
    // }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate/*, GameMode gameMode*/)
    {
        Player player = GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject> {
                // { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) },
                { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
            }
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        // GameSceneManager.Instance.LoadNextScene();
        GameSceneManager.Instance.LoadScene("Lobby Room");
        // Debug.Log("Created Lobby " + lobby.Name);
    }

    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    value: "0",
                    op: QueryFilter.OpOptions.GT
                    )
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
        {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        // GameSceneManager.Instance.LoadNextScene();
        GameSceneManager.Instance.LoadScene("Lobby Room");
    }

    public string GetJoinedLobbyCode()
    {
        return joinedLobbyCode;
    }

    public async void JoinLobby(Lobby lobby)
    {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
        {
            Player = player
        });

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        // GameSceneManager.Instance.LoadNextScene();
        GameSceneManager.Instance.LoadScene("Lobby Room");
    }

    public async void UpdatePlayerName(string name)
    {

        // update player name
        playerName = name;

        // if player in lobby
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                // update player name in options variable
                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                // find playerID in lobby
                string playerId = AuthenticationService.Instance.PlayerId;

                // update
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter)
    // {
    //     if (joinedLobby != null)
    //     {
    //         try
    //         {
    //             UpdatePlayerOptions options = new UpdatePlayerOptions();

    //             options.Data = new Dictionary<string, PlayerDataObject>() {
    //                 {
    //                     KEY_PLAYER_CHARACTER, new PlayerDataObject(
    //                         visibility: PlayerDataObject.VisibilityOptions.Public,
    //                         value: playerCharacter.ToString())
    //                 }
    //             };

    //             string playerId = AuthenticationService.Instance.PlayerId;

    //             Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
    //             joinedLobby = lobby;

    //             OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
    //         }
    //         catch (LobbyServiceException e)
    //         {
    //             Debug.Log(e);
    //         }
    //     }
    // }

    // public async void QuickJoinLobby()
    // {
    //     try
    //     {
    //         QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

    //         Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
    //         joinedLobby = lobby;

    //         OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    //     }
    //     catch (LobbyServiceException e)
    //     {
    //         Debug.Log(e);
    //     }
    // }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);

                //GameSceneManager.Instance.LoadPreviousScene();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // public async void UpdateLobbyGameMode(GameMode gameMode)
    // {
    //     try
    //     {
    //         Debug.Log("UpdateLobbyGameMode " + gameMode);

    //         Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
    //         {
    //             Data = new Dictionary<string, DataObject> {
    //                 { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
    //             }
    //         });

    //         joinedLobby = lobby;

    //         OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
    //     }
    //     catch (LobbyServiceException e)
    //     {
    //         Debug.Log(e);
    //     }
    // }

    public async void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                // Debug.Log("StartGame");

                string relayCode = await Relay.Instance.CreateRelay(joinedLobby.Players.Count);

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject> {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });

                joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void ExitGame()
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                    }
            });

            joinedLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}