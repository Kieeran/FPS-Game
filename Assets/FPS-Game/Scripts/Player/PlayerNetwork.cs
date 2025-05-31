using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System;

public class PlayerNetwork : NetworkBehaviour, IInitAwake, IInitStart, IInitNetwork
{
    [HideInInspector]
    public string playerName = "Playername";
    [HideInInspector]
    public NetworkVariable<int> KillCount = new();
    [HideInInspector]
    public NetworkVariable<int> DeathCount = new();

    public PlayerRoot PlayerRoot { get; private set; }

    public float RespawnDelay;

    private GameObject[] spawnerList;

    public struct PlayerInfo
    {
        public string PlayerName;
        public int KillCount;
        public int DeathCount;

        public PlayerInfo(string name, int killCount, int deathCount)
        {
            PlayerName = name;
            KillCount = killCount;
            DeathCount = deathCount;
        }
    }

    List<PlayerInfo> _playerInfos;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
        _playerInfos = new List<PlayerInfo>();
    }

    // Start
    public int PriorityStart => 1000;
    public void InitializeStart()
    {

    }

    // OnNetworkSpawn
    public int PriorityNetwork => 5;
    public void InitializeOnNetworkSpawn()
    {
        if (IsOwner == false) return;

        SpawnPosition randomPos = GameManager.Instance.GetRandomPos();

        Debug.Log($"Spawn at {randomPos.gameObject.name}: {randomPos.SpawnPos} {randomPos.SpawnRot.eulerAngles}");
        transform.position = randomPos.SpawnPos;
        // transform.GetChild(0).rotation = randomPos.SpawnRot;

        KillCount = new();
        DeathCount = new();

        EnableScripts();
        MappingValues_ServerRpc(AuthenticationService.Instance.PlayerId, OwnerClientId);

        // SetRandomPosAtSpawn_ServerRpc(OwnerClientId);

        SetCinemachineVirtualCamera();

        PlayerRoot.PlayerTakeDamage.PlayerDead += OnPlayerDead;

        PlayerRoot.PlayerModel.HideHead();
    }

    void OnDisable()
    {
        PlayerRoot.PlayerTakeDamage.PlayerDead -= OnPlayerDead;
    }

    void SetCinemachineVirtualCamera()
    {
        CinemachineVirtualCamera _camera = GameManager.Instance.GetCinemachineVirtualCamera();
        if (_camera != null)
        {
            Transform playerCamera = null;

            foreach (Transform child in transform)
            {
                if (child.CompareTag("CinemachineTarget"))
                {
                    playerCamera = child;
                    break;
                }
            }

            if (playerCamera != null) _camera.Follow = playerCamera;
            if (_camera.Follow == null) Debug.Log("_camera.Follow = null");
        }
    }

    void RemoveCinemachineVirtualCamera()
    {
        CinemachineVirtualCamera _camera = GameManager.Instance.GetCinemachineVirtualCamera();
        if (_camera != null)
        {
            Transform playerCameraRoot = transform.Find("PlayerCameraRoot");

            if (playerCameraRoot != null) _camera.Follow = null;
        }
    }

    #region =========================================At Spawn=========================================
    [ServerRpc(RequireOwnership = false)]
    void SetRandomPosAtSpawn_ServerRpc(ulong clientId)
    {
        SpawnPosition randomPos = GameManager.Instance.GetRandomPos();
        if (randomPos == null)
        {
            Debug.Log("Null");
            return;
        }

        Debug.Log($"Spawn at {randomPos.gameObject.name}: {randomPos.SpawnPos} {randomPos.SpawnRot.eulerAngles}");

        SetRandomPosAtSpawn_ClientRpc(
            randomPos.SpawnPos,
            randomPos.SpawnRot,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { clientId }
                }
            }
        );
    }

    [ClientRpc]
    void SetRandomPosAtSpawn_ClientRpc(Vector3 randomPos, Quaternion rot, ClientRpcParams clientRpcParams)
    {
        PlayerRoot.CharacterController.enabled = false;
        PlayerRoot.PlayerController.enabled = false;

        PlayerRoot.ClientNetworkTransform.Interpolate = false;

        transform.position = randomPos;
        // transform.GetChild(0).rotation = rot;

        Invoke(nameof(EnableInterpolationAtSpawn), 0.1f);

        PlayerRoot.CharacterController.enabled = true;
        PlayerRoot.PlayerController.enabled = true;
    }

    void EnableInterpolationAtSpawn()
    {
        if (PlayerRoot.ClientNetworkTransform != null)
        {
            PlayerRoot.ClientNetworkTransform.Interpolate = true;
        }
    }
    #endregion ============================================================================================

    [ServerRpc(RequireOwnership = false)]
    void SetRandomPos_ServerRpc(ulong clientId)
    {
        SpawnPosition randomPos = GameManager.Instance.GetRandomPos();
        if (randomPos == null)
        {
            Debug.Log("Null");
            return;
        }

        Debug.Log($"Spawn at {randomPos.gameObject.name}: {randomPos.SpawnPos} {randomPos.SpawnRot.eulerAngles}");

        SetRandomPos_ClientRpc(
            randomPos.SpawnPos,
            randomPos.SpawnRot,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { clientId }
                }
            }
        );
    }

    [ClientRpc]
    void SetRandomPos_ClientRpc(Vector3 randomPos, Quaternion rot, ClientRpcParams clientRpcParams)
    {
        PlayerRoot.PlayerModel.OnPlayerRespawn();

        PlayerRoot.ClientNetworkTransform.Interpolate = false;

        transform.position = randomPos;
        // transform.GetChild(0).rotation = rot;

        Invoke(nameof(EnableInterpolation), 0.1f);

        PlayerRoot.CharacterController.enabled = true;
        PlayerRoot.PlayerController.enabled = true;
    }

    void EnableInterpolation()
    {
        if (PlayerRoot.ClientNetworkTransform != null)
        {
            PlayerRoot.ClientNetworkTransform.Interpolate = true;

            PlayerRoot.PlayerTakeDamage.ResetPlayerHP_ServerRpc(OwnerClientId);
        }
    }

    void RequestSetRandomPos()
    {
        SetRandomPos_ServerRpc(OwnerClientId);
    }

    void OnPlayerDead()
    {
        PlayerRoot.CharacterController.enabled = false;
        PlayerRoot.PlayerController.enabled = false;

        DeadAnimation();

        Invoke(nameof(RequestSetRandomPos), RespawnDelay);
    }

    void DeadAnimation()
    {
        RemoveCinemachineVirtualCamera();

        PlayerRoot.PlayerModel.OnPlayerDie();
        PlayerRoot.WeaponHolder.DropWeapon();

        Invoke(nameof(Respawn), RespawnDelay);
    }

    void Respawn()
    {
        PlayerRoot.WeaponHolder.ResetWeaponHolder();
        SetCinemachineVirtualCamera();
    }

    void EnableScripts()
    {
        PlayerRoot.PlayerInput.enabled = true;
        PlayerRoot.CharacterController.enabled = true;
        PlayerRoot.PlayerController.enabled = true;
        PlayerRoot.PlayerShoot.enabled = true;
        PlayerRoot.PlayerUI.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void MappingValues_ServerRpc(string playerID, ulong targetClientId)
    {
        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();
        foreach (Player player in lobby.Players)
        {
            if (player.Id == playerID)
            {
                var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
                if (targetPlayer.TryGetComponent<PlayerNetwork>(out var playerNetwork))
                {
                    playerNetwork.playerName = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
                    return;
                }
            }
        }
    }

    void Update()
    {
        if (IsOwner == false) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            SetRandomPosAtSpawn_ServerRpc(OwnerClientId);
            // OnPlayerDead();
            // GetAllPlayerInfos_ServerRPC(OwnerClientId);
        }
    }

    public List<PlayerInfo> GetAllPlayerInfos()
    {
        GetAllPlayerInfos_ServerRPC(OwnerClientId);
        return _playerInfos;
    }

    [ServerRpc(RequireOwnership = false)]
    void GetAllPlayerInfos_ServerRPC(ulong clientID)
    {
        string result = "";

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.TryGetComponent<PlayerNetwork>(out var playerNetwork))
            {
                result += $"{playerNetwork.playerName};{playerNetwork.KillCount.Value};{playerNetwork.DeathCount.Value}|";
            }
        }

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { clientID }
            }
        };

        GetAllPlayerInfos_ClientRPC(result, clientRpcParams);
    }

    [ClientRpc]
    void GetAllPlayerInfos_ClientRPC(string data, ClientRpcParams clientRpcParams)
    {
        string[] playerEntries = data.Split('|', StringSplitOptions.RemoveEmptyEntries);

        _playerInfos?.Clear();

        foreach (string entry in playerEntries)
        {
            string[] tokens = entry.Split(';');
            if (tokens.Length == 3)
            {
                string name = tokens[0];
                int kill = int.Parse(tokens[1]);
                int death = int.Parse(tokens[2]);
                _playerInfos.Add(new PlayerInfo(name, kill, death));

                Debug.Log($"Name: {name}, Kill: {kill}, Death: {death}");
            }
        }
    }
}