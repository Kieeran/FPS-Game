using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PlayerAssets;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public PlayerInput playerInput;
    public CharacterController characterController;
    public PlayerController playerController;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner == true)
        {
            playerInput.enabled = true;
            characterController.enabled = true;
            playerController.enabled = true;
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
        //Debug.Log(OwnerClientId + " randomNumber: " + randomNumber.Value);

        //playerInput.enabled = IsOwner;

        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     randomNumber.Value = Random.Range(0, 100);
        // }
    }
}
