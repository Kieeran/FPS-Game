using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{
    //public PlayerNetwork playerNetwork;
    public PlayerTakeDamage playerTakeDamage;

    public void Hit()
    {
        //playerNetwork.TakeDamage(0.05f);
        //UIManager.Instance.UpdateHealth(0.05f);
        //     playerTakeDamage.TakeDamage(0.05f);

        //     Debug.Log("Hit " + OwnerClientId + " body");
    }
}