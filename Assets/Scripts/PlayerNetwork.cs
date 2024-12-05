using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    void Update()
    {
        //Debug.Log(OwnerClientId + " randomNumber: " + randomNumber.Value);

        if (IsOwner == false) return;

        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     randomNumber.Value = Random.Range(0, 100);
        // }
    }
}
