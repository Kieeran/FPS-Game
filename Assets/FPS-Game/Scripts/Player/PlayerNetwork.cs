using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class PlayerNetwork : NetworkBehaviour
{
    [HideInInspector]
    public string playerName = "Playername";
    [HideInInspector]
    public NetworkVariable<int> KillCount = new();
    [HideInInspector]
    public NetworkVariable<int> DeathCount = new();

    public PlayerRoot PlayerRoot { get; private set; }

    public Image health;
    public Canvas playerUI;

    public float RespawnDelay;

    public struct PlayerInfo
    {
        public string Name;
        public int KillCount;
        public int DeathCount;

        public PlayerInfo(string name, int killCount, int deathCount)
        {
            Name = name;
            KillCount = killCount;
            DeathCount = deathCount;
        }
    }

    public PlayerHead PlayerHead;
    public PlayerBody PlayerBody;

    Vector3 originPosHead;
    Quaternion originRotHead;

    Vector3 originPosBody;
    Quaternion originRotBody;

    void Awake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();

        SetOrigin();
    }

    void Start()
    {
        PlayerHead.Rb.isKinematic = true;
        PlayerBody.Rb.isKinematic = true;
    }

    void SetOrigin()
    {
        originPosHead = PlayerHead.transform.localPosition;
        originRotHead = PlayerHead.transform.localRotation;

        originPosBody = PlayerBody.transform.localPosition;
        originRotBody = PlayerBody.transform.localRotation;
    }

    void ResetParts()
    {
        PlayerHead.transform.SetLocalPositionAndRotation(originPosHead, originRotHead);
        PlayerBody.transform.SetLocalPositionAndRotation(originPosBody, originRotBody);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner == false) return;

        EnableScripts();
        MappingValues_ServerRpc(AuthenticationService.Instance.PlayerId, OwnerClientId);

        SetRandomPosAtSpawn_ServerRpc(OwnerClientId);

        SetCinemachineVirtualCamera();

        PlayerRoot.PlayerUI.OnOpenScoreBoard += OnOpenScoreBoard;
        PlayerRoot.PlayerTakeDamage.PlayerDead += OnPlayerDead;
    }

    void SetCinemachineVirtualCamera()
    {
        CinemachineVirtualCamera _camera = GameManager.Instance.GetCinemachineVirtualCamera();
        if (_camera != null)
        {
            Transform playerCameraRoot = transform.Find("PlayerCameraRoot");

            if (playerCameraRoot != null) _camera.Follow = playerCameraRoot;
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
        Transform randomPos = GameManager.Instance.GetRandomPos();
        if (randomPos == null)
        {
            Debug.Log("Null");
            return;
        }

        SetRandomPosAtSpawn_ClientRpc(
            randomPos.position,
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
    void SetRandomPosAtSpawn_ClientRpc(Vector3 randomPos, ClientRpcParams clientRpcParams)
    {
        PlayerRoot.CharacterController.enabled = false;
        PlayerRoot.PlayerController.enabled = false;

        PlayerRoot.ClientNetworkTransform.Interpolate = false;

        transform.position = randomPos;

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
        Transform randomPos = GameManager.Instance.GetRandomPos();
        if (randomPos == null)
        {
            Debug.Log("Null");
            return;
        }

        SetRandomPos_ClientRpc(
            randomPos.position,
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
    void SetRandomPos_ClientRpc(Vector3 randomPos, ClientRpcParams clientRpcParams)
    {
        PlayerRoot.ClientNetworkTransform.Interpolate = false;

        transform.position = randomPos;

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

        RemoveCinemachineVirtualCamera();
        AddRig();
        Invoke(nameof(RestartModel), RespawnDelay);

        Invoke(nameof(RequestSetRandomPos), RespawnDelay);
    }

    void EnableScripts()
    {
        PlayerRoot.PlayerInput.enabled = true;
        PlayerRoot.CharacterController.enabled = true;
        PlayerRoot.PlayerController.enabled = true;
        PlayerRoot.PlayerShoot.enabled = true;
        PlayerRoot.PlayerUI.enabled = true;

        playerUI.gameObject.SetActive(true);
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

    void AddRig()
    {
        PlayerHead.Rb.isKinematic = false;
        PlayerBody.Rb.isKinematic = false;
    }

    void RestartModel()
    {
        PlayerHead.Rb.isKinematic = true;
        PlayerBody.Rb.isKinematic = true;
        
        ResetParts();
        SetCinemachineVirtualCamera();
    }

    void Update()
    {
        if (IsOwner == false) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            RemoveCinemachineVirtualCamera();
            AddRig();
            Invoke(nameof(RestartModel), 4f);
            // GetAllPlayerInfos_ServerRPC(OwnerClientId);
        }
    }

    void OnOpenScoreBoard()
    {
        GetAllPlayerInfos();
    }

    public void GetAllPlayerInfos()
    {
        GetAllPlayerInfos_ServerRPC(OwnerClientId);
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

        List<PlayerInfo> playerInfos = new();

        foreach (string entry in playerEntries)
        {
            string[] tokens = entry.Split(';');
            if (tokens.Length == 3)
            {
                string name = tokens[0];
                int kill = int.Parse(tokens[1]);
                int death = int.Parse(tokens[2]);
                playerInfos.Add(new PlayerInfo(name, kill, death));

                Debug.Log($"Name: {name}, Kill: {kill}, Death: {death}");
            }
        }

        PlayerRoot.PlayerUI.AddInfoToScoreBoard(playerInfos);
    }
}