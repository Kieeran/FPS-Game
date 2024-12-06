using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public PlayerInput playerInput;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerInput.enabled = IsOwner;
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
