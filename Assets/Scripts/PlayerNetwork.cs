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

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // public TMP_Text tMP_Text;

    public PlayerInput playerInput;
    public CharacterController characterController;
    public PlayerController playerController;
    public PlayerShoot playerShoot;
    public PlayerTakeDamage playerTakeDamage;

    public Image health;

    public GameObject playerUI;

    private void EnableScripts()
    {
        playerInput.enabled = true;
        characterController.enabled = true;
        playerController.enabled = true;
        playerShoot.enabled = true;
        playerTakeDamage.enabled = true;

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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner == true)
        {
            EnableScripts();

            CinemachineVirtualCamera _camera = GameManager.Instance.playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
            if (_camera != null)
            {
                Transform playerCameraRoot = transform.Find("PlayerCameraRoot");

                if (playerCameraRoot != null) _camera.Follow = playerCameraRoot;

                Debug.Log(playerCameraRoot.name);
            }
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

    // [ServerRpc(RequireOwnership = false)]
    // [ServerRpc]
    // public void ChangeRandomNumberServerRpc(int num, ulong targetClientId)
    // {
    //     // Tìm đối tượng của client mục tiêu
    //     var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
    //     if (targetPlayer.TryGetComponent<PlayerNetwork>(out var player))
    //     {
    //         player.randomNumber.Value = num;
    //     }
    // }
}