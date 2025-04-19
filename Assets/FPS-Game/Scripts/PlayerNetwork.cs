using Cinemachine;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Collections;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;

public class PlayerNetwork : NetworkBehaviour
{
    // private NetworkVariable<FixedString32Bytes> playerName = new("Playername");
    public string playerName = "Playername";
    public NetworkVariable<int> killCount = new(0);
    public NetworkVariable<int> deathCount = new(0);

    public List<PlayerNetwork> playerList = new();

    public PlayerInput playerInput;
    public CharacterController characterController;
    public PlayerController playerController;
    public PlayerShoot playerShoot;
    public PlayerTakeDamage playerTakeDamage;
    public PlayerUI _playerUI;

    public Image health;

    public Canvas playerUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // if (!IsClient)
        // {
        //     foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        //     {
        //         NetworkObject player = client.PlayerObject;

        //         if (player.TryGetComponent<PlayerNetwork>(out var playerNetwork))
        //         {
        //             // Debug.Log(playerNetwork.playerName);
        //             playerList.Add(playerNetwork);
        //         }
        //     }
        // }

        // else
        // {
        //     PrintPlayerName_ServerRPC();
        // }

        if (IsOwner == true)
        {
            EnableScripts();
            MappingValues_ServerRpc(AuthenticationService.Instance.PlayerId, OwnerClientId);


            CinemachineVirtualCamera _camera = GameManager.Instance.GetCinemachineVirtualCamera();
            if (_camera != null)
            {
                Transform playerCameraRoot = transform.Find("PlayerCameraRoot");

                if (playerCameraRoot != null) _camera.Follow = playerCameraRoot;
            }
        }
    }

    private void EnableScripts()
    {
        playerInput.enabled = true;
        characterController.enabled = true;
        playerController.enabled = true;
        playerShoot.enabled = true;
        playerTakeDamage.enabled = true;
        _playerUI.enabled = true;

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
                    playerList.Add(playerNetwork);
                    return;
                }
            }
        }
    }

    void Update()
    {

        // if (!IsOwner) return;


        if (Input.GetKeyDown(KeyCode.T))
        {
            PrintPlayerName_ServerRPC();
            // foreach (PlayerNetwork playerNetwork in playerList)
            // {
            //     if (playerList == null)
            //     {
            //         Debug.Log("Player List is null");
            //     }
            //     else Debug.Log("Player List: " + playerList);
            //     // Debug.Log("Player name: " + playerNetwork.playerName);
            // }
            //     if (!IsClient)
            //     {
            //         foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            //         {
            //             NetworkObject player = client.PlayerObject;

            //             if (player.TryGetComponent<PlayerNetwork>(out var playerNetwork))
            //             {
            //                 // Debug.Log(playerNetwork.playerName);
            //                 playerList.Add(playerNetwork);
            //             }
            //         }
            //     }

            //     else
            //     {
            //         PrintPlayerName_ServerRPC();
            //     }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PrintPlayerName_ServerRPC()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;

            if (player.TryGetComponent<PlayerNetwork>(out var playerNetwork))
            {
                // PrintPlayerName_ClientRPC(playerNetwork.playerName);
                playerList.Add(playerNetwork);
                AddPlayerNetwork_ClientRpc(playerNetwork.playerName, playerNetwork.killCount.Value, playerNetwork.deathCount.Value);
            }
        }
    }

    // [ClientRpc]
    // void PrintPlayerName_ClientRPC(FixedString32Bytes playerName)
    // {
    //     // Debug.Log(playerName);
    // }

    [ClientRpc]
    void AddPlayerNetwork_ClientRpc(string playerName, int killCount, int deathCount)
    {
        Debug.Log("Player's name - kill/death: " + playerName + " - " + killCount + "/" + deathCount);
    }
}