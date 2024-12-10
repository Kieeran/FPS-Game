using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTakeDamage : NetworkBehaviour
{
    private NetworkVariable<float> HP = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> damage = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private Image health;

    public void TakeDamage(float amount)
    {
        Debug.Log(OwnerClientId + " take damage");

        HP.Value -= amount;
        if (HP.Value == 0) HP.Value = 1;

        health.fillAmount = HP.Value;

        Debug.Log(OwnerClientId + " current HP: " + HP.Value);
    }

    [ServerRpc]
    public void ChangeHPServerRpc(float damage, ulong targetClientId)
    {
        // Tìm đối tượng của client mục tiêu
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            targetHealth.HP.Value -= damage;

            if (targetHealth.HP.Value <= 0) targetHealth.HP.Value = 1;
        }
    }
}