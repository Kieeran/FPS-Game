using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTakeDamage : NetworkBehaviour
{
    private NetworkVariable<float> HP = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Image health;

    public void TakeDamage(float damage, ulong targetClientId)
    {
        Debug.Log($"{OwnerClientId} take {damage} damage");
        ChangeHPServerRpc(damage, targetClientId);
    }

    private void Update()
    {
        if (!IsOwner) return;

        health.fillAmount = HP.Value;
        //Debug.Log(OwnerClientId + " current HP: " + HP.Value);
    }

    public void UpdateUI()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(float damage, ulong targetClientId)
    {
        // Tìm đối tượng của client mục tiêu
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            targetHealth.HP.Value -= damage;

            if (targetHealth.HP.Value <= 0) targetHealth.HP.Value = 1;

            Debug.Log($"{targetClientId} current HP: {targetHealth.HP.Value}");
        }
    }
}