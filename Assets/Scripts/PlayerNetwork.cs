using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PlayerAssets;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using TMPro;
using Unity.Collections;

public class PlayerNetwork : NetworkBehaviour
{
    // private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // private NetworkVariable<string> playerName = new NetworkVariable<string>("Playername", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // private NetworkVariable<int> killCount = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // private NetworkVariable<int> deathCount = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("Playername", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> killCount = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> deathCount = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // public TMP_Text tMP_Text;

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

        if (IsOwner == true)
        {
            EnableScripts();
            InitializeValues(OwnerClientId);

            CinemachineVirtualCamera _camera = GameManager.Instance.GetCinemachineVirtualCamera();
            if (_camera != null)
            {
                Transform playerCameraRoot = transform.Find("PlayerCameraRoot");

                if (playerCameraRoot != null) _camera.Follow = playerCameraRoot;

                Debug.Log(playerCameraRoot.name);
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

    public void TakeDamage(float amount)
    {
        Debug.Log("Hit " + OwnerClientId);
        // if (IsOwner == true)
        // {
        //     Debug.Log(OwnerClientId + " take damage");

        //     health.fillAmount -= amount;
        //     if (health.fillAmount == 0)
        //     {
        //         health.fillAmount = 1;
        //     }
        // }

        Debug.Log(OwnerClientId + " take damage");

        health.fillAmount -= amount;
        if (health.fillAmount == 0)
        {
            health.fillAmount = 1;
        }
    }

    private void InitializeValues(ulong targetClientId)
    {
        InitializeValues_ServerRpc(
            LobbyManager.Instance.GetPlayerName(),
            0,
            0,
            targetClientId
        );

        // if (!IsOwner) return;

        // //playerName.Value = LobbyManager.Instance.GetPlayerName();
        // killCount.Value = 2;
        // deathCount.Value = 12;
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeValues_ServerRpc(string playerName, int killCount, int deathCount, ulong targetClientId)
    {
        // Tìm đối tượng của client mục tiêu
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerNetwork>(out var player))
        {
            this.playerName.Value = playerName;
            this.killCount.Value = killCount;
            this.deathCount.Value = deathCount;
        }
    }

    void Update()
    {
        ////Debug.Log(OwnerClientId + " randomNumber: " + HP.Value);

        //playerInput.enabled = IsOwner;

        //tMP_Text.text = randomNumber.Value.ToString();

        // if (IsOwner)
        // {
        //     if (Input.GetKeyDown(KeyCode.T))
        //     {
        //         // randomNumber.Value = Random.Range(0, 100);
        //         ChangeRandomNumberServerRpc(Random.Range(0, 100), transform.GetComponent<NetworkObject>().OwnerClientId);

        //         Debug.Log(OwnerClientId + " randomNumber: " + randomNumber.Value);
        //     }

        //     if (Input.GetKeyDown(KeyCode.Y))
        //     {
        //         if (transform.GetComponent<NetworkObject>().OwnerClientId == 0)
        //             ChangeRandomNumberServerRpc(Random.Range(0, 100), 1);

        //         else
        //             ChangeRandomNumberServerRpc(Random.Range(0, 100), 0);
        //     }
        // }
    }
}