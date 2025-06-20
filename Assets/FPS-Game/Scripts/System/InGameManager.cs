using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using Unity.Netcode;

public struct PlayerInfo
{
    public ulong PlayerId;
    public string PlayerName;
    public int KillCount;
    public int DeathCount;

    public PlayerInfo(ulong playerId, string name, int killCount, int deathCount)
    {
        PlayerId = playerId;
        PlayerName = name;
        KillCount = killCount;
        DeathCount = deathCount;
    }
}

public class InGameManager : NetworkBehaviour
{
    [SerializeField] CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] Transform _spawnPositions;

    public static InGameManager Instance { get; private set; }
    public CinemachineVirtualCamera GetCinemachineVirtualCamera() { return _cinemachineVirtualCamera; }
    public List<SpawnPosition> SpawnPositionsList { get; private set; }
    public TimePhaseCounter TimePhaseCounter { get; private set; }
    public KillCountChecker KillCountChecker { get; private set; }
    public GenerateHealthPickup GenerateHealthPickup { get; private set; }

    public System.Action OnGameEnd;

    public bool IsGameEnd = false;
    public NetworkVariable<bool> IsTimeOut = new();

    public System.Action<List<PlayerInfo>> OnReceivedPlayerInfo;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitSpawnPositions();
        TimePhaseCounter = GetComponent<TimePhaseCounter>();
        KillCountChecker = GetComponent<KillCountChecker>();
        GenerateHealthPickup = GetComponent<GenerateHealthPickup>();

        OnGameEnd += () =>
        {
            IsGameEnd = true;
        };
    }

    void InitSpawnPositions()
    {
        SpawnPositionsList = new List<SpawnPosition>();
        foreach (Transform child in _spawnPositions)
        {
            SpawnPositionsList.Add(child.GetComponent<SpawnPosition>());
        }
    }

    public SpawnPosition GetRandomPos()
    {
        if (SpawnPositionsList == null || SpawnPositionsList.Count == 0)
        {
            Debug.LogError("SpawnPositionsList is empty!");
            return null;
        }

        return SpawnPositionsList[Random.Range(0, SpawnPositionsList.Count)];
    }

    public void GetAllPlayerInfos()
    {
        GetAllPlayerInfos_ServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    void GetAllPlayerInfos_ServerRPC(ServerRpcParams rpcParams = default)
    {
        // Chỉ chạy đoạn này nếu là server
        if (!NetworkManager.Singleton.IsServer) return;

        string result = "";
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.TryGetComponent<PlayerNetwork>(out var playerNetwork))
            {
                result += $"{playerNetwork.OwnerClientId};{playerNetwork.playerName};{playerNetwork.KillCount.Value};{playerNetwork.DeathCount.Value}|";
            }
        }

        // Gửi kết quả về đúng client đã yêu cầu
        ulong requestingClientId = rpcParams.Receive.SenderClientId;
        ClientRpcParams clientRpcParams = new()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { requestingClientId }
            }
        };

        GetAllPlayerInfos_ClientRPC(result, clientRpcParams);
    }

    [ClientRpc]
    void GetAllPlayerInfos_ClientRPC(string data, ClientRpcParams clientRpcParams = default)
    {
        List<PlayerInfo> playerInfos = new();
        string[] playerEntries = data.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string entry in playerEntries)
        {
            string[] tokens = entry.Split(';');
            if (tokens.Length == 4)
            {
                ulong id = ulong.Parse(tokens[0]);
                string name = tokens[1];
                int kill = int.Parse(tokens[2]);
                int death = int.Parse(tokens[3]);
                playerInfos.Add(new PlayerInfo(id, name, kill, death));

                Debug.Log($"Id: {id}, Name: {name}, Kill: {kill}, Death: {death}");
            }
        }
        OnReceivedPlayerInfo?.Invoke(playerInfos);
    }
}