using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.Collections;

public class PlayerNetwork : PlayerBehaviour
{
    [HideInInspector]
    public string playerName = "Playername";
    [HideInInspector]
    public NetworkVariable<int> KillCount = new(0);
    [HideInInspector]
    public NetworkVariable<int> DeathCount = new(0);

    public float RespawnDelay;

    // OnNetworkSpawn
    public override int PriorityNetwork => 5;
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        if (IsOwner)
        {
            if (IsServer && gameObject.name == "Bot")
            {
                PlayerRoot.IsBot.Value = true;
            }
            Debug.Log(PlayerRoot.IsBot.Value);
            Debug.Log(gameObject.name);

            if (!PlayerRoot.IsBot.Value && IsServer)
            {
                // SpawnBot();
            }
            EnableScripts();
            if (gameObject.name != "Bot" || !PlayerRoot.IsBot.Value)
            {
                MappingValues_ServerRpc(AuthenticationService.Instance.PlayerId, OwnerClientId);
                PlayerRoot.PlayerModel.ChangeModelVisibility(false);
            }
            PlayerRoot.Events.OnPlayerDead += OnPlayerDead;
        }
        else
        {
            if (PlayerRoot.IsBot.Value)
            {
                SyncBotNetwork();
            }
        }
    }

    void OnDisable()
    {
        PlayerRoot.Events.OnPlayerDead -= OnPlayerDead;
    }

    public override void OnInGameManagerReady(InGameManager manager)
    {
        base.OnInGameManagerReady(manager);
        if (IsOwner)
        {
            StartCoroutine(SpawnRandom());
            if (gameObject.name != "Bot") SetCinemachineVirtualCamera();
        }
    }

    void SetCinemachineVirtualCamera()
    {
        CinemachineVirtualCamera _camera = InGameManager.Instance.PlayerFollowCamera;
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
        CinemachineVirtualCamera _camera = InGameManager.Instance.PlayerFollowCamera;
        if (_camera != null)
        {
            Transform playerCameraRoot = transform.Find("PlayerCameraRoot");

            if (playerCameraRoot != null) _camera.Follow = null;
        }
    }

    void SyncBotNetwork()
    {
        SyncBotNetwork_InspectorName();
        SyncBotNetwork_Component();
    }

    void SyncBotNetwork_InspectorName()
    {
        gameObject.name = "Bot";
    }

    void SyncBotNetwork_Component()
    {
        PlayerRoot.PlayerCamera.enabled = false;
        PlayerRoot.PlayerModel.ChangeRigBuilderState(false);
    }

    #region =========================================At Spawn=========================================
    IEnumerator SpawnRandom()
    {
        yield return null;
        SpawnPosition randomPos = InGameManager.Instance.GetRandomPos();

        Debug.Log($"Spawn at {randomPos.gameObject.name}: {randomPos.SpawnPos} {randomPos.SpawnRot.eulerAngles}");
        transform.position = randomPos.SpawnPos;
        transform.GetChild(0).rotation = randomPos.SpawnRot;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetRandomPosAtSpawn_ServerRpc(ulong clientId)
    {
        SpawnPosition randomPos = InGameManager.Instance.GetRandomPos();
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
        PlayerRoot.PlayerController.RotateCameraTo(rot);

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
        SpawnPosition randomPos = InGameManager.Instance.GetRandomPos();
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
        PlayerRoot.PlayerController.RotateCameraTo(rot);

        Invoke(nameof(EnableInterpolation), 0.1f);

        PlayerRoot.CharacterController.enabled = true;
        PlayerRoot.PlayerController.enabled = true;

        PlayerRoot.PlayerUI.CurrentPlayerCanvas.HitEffect.ResetHitEffect();
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
        PlayerRoot.Events.InvokeOnPlayerRespawn();
    }

    void EnableScripts()
    {
        PlayerRoot.CharacterController.enabled = true;
        PlayerRoot.PlayerController.enabled = true;
        PlayerRoot.PlayerShoot.enabled = true;

        if (!PlayerRoot.IsBot.Value)
        {
            PlayerRoot.PlayerInput.enabled = true;
            PlayerRoot.PlayerUI.enabled = true;
        }

        else
        {
            PlayerRoot.PlayerInput.enabled = false;
            PlayerRoot.PlayerUI.enabled = false;
            PlayerRoot.PlayerCamera.enabled = false;
        }
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

    // void Update()
    // {
    //     if (IsOwner == false) return;

    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         SetRandomPosAtSpawn_ServerRpc(OwnerClientId);
    //         OnPlayerDead();
    //         GetAllPlayerInfos_ServerRPC(OwnerClientId);
    //     }
    // }
}