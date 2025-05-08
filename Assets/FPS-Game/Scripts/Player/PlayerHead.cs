using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerHead : NetworkBehaviour
{
    //public PlayerNetwork playerNetwork;
    public PlayerTakeDamage playerTakeDamage;

    public void Hit()
    {
        //playerNetwork.TakeDamage(0.1f);
        //UIManager.Instance.UpdateHealth(0.1f);

        // playerTakeDamage.TakeDamage(0.1f);

        // Debug.Log("Hit " + OwnerClientId + " head");
    }
}