using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerTakeDamage : NetworkBehaviour, IInitAwake, IInitNetwork
{
    public PlayerRoot PlayerRoot { get; private set; }

    [HideInInspector]
    public NetworkVariable<float> HP = new(1);

    public Action PlayerDead;
    public bool IsPlayerDead = false;


    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    // OnNetworkSpawn
    public int PriorityNetwork => 15;
    public void InitializeOnNetworkSpawn()
    {
        HP.OnValueChanged += OnHPChanged;
    }

    private void OnHPChanged(float previous, float current)
    {
        if (previous == current) return;

        if (IsOwner)
            PlayerRoot.PlayerUI.CurrentPlayerCanvas.HealthBar.UpdatePlayerHealthBar(current);

        // if (previous == 0) IsPlayerDead = false;

        // if (current == 0)
        // {
        //     IsPlayerDead = true;
        //     PlayerDead?.Invoke();
        // }
    }

    public void TakeDamage(float damage, string shotType, ulong targetClientId, ulong ownerPlayerID)
    {
        ChangeHPServerRpc(damage, shotType, targetClientId, ownerPlayerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(float damage, string shotType, ulong targetClientId, ulong ownerClientId)
    {
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        var ownerPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            if (targetHealth.HP.Value == 0) return;

            targetHealth.HP.Value -= damage;
            if (targetHealth.HP.Value <= 0)
            {
                if (targetPlayer.TryGetComponent<PlayerNetwork>(out var clientPlayerNetwork))
                {
                    clientPlayerNetwork.DeathCount.Value += 1;
                }
                if (ownerPlayer.TryGetComponent<PlayerNetwork>(out var ownerPlayerNetwork))
                {
                    ownerPlayerNetwork.KillCount.Value += 1;
                }
                // targetHealth.HP.Value = 0;
                targetHealth.HP.Value = 1;
            }

            Debug.Log($"{targetClientId} current HP: {targetHealth.HP.Value}");
        }

        PlayerRoot.PlayerUI.AddTakeDamageEffect(shotType, targetClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetPlayerHP_ServerRpc(ulong ownerClientId)
    {
        var ownerPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject;
        if (ownerPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            targetHealth.HP.Value = 1;
        }
    }
}