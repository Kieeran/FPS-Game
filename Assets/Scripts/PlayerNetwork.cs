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
    // private NetworkVariable<float> HP = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

            CinemachineVirtualCamera _camera = TestRelay.Instance.playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
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

        // if (IsOwner)
        // {
        //     if (Input.GetKeyDown(KeyCode.T))
        //     {
        //         HP.Value = Random.Range(0, 100);

        //         Debug.Log(OwnerClientId + " randomNumber: " + HP.Value);
        //         tMP_Text.text = HP.Value.ToString();
        //     }
        // }
    }
}
