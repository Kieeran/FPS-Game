using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerTakeDamage : PlayerBehaviour
{
    [HideInInspector]
    public NetworkVariable<float> HP = new(1);

    public Action OnPlayerDead;
    public bool IsPlayerDead = false;

    // OnNetworkSpawn
    public override int PriorityNetwork => 15;
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        HP.OnValueChanged += OnHPChanged;
    }

    private void OnHPChanged(float previous, float current)
    {
        if (previous == current) return;

        if (IsOwner)
            PlayerRoot.PlayerUI.CurrentPlayerCanvas.HealthBar.UpdatePlayerHealthBar(current);

        if (previous == 0) IsPlayerDead = false;

        if (current == 0)
        {
            IsPlayerDead = true;
            OnPlayerDead?.Invoke();
            InGameManager.Instance.GenerateHealthPickup.DropHealthPickup(transform.position);
        }
    }

    public void TakeDamage(float damage, ulong targetClientId, ulong ownerPlayerID)
    {
        ChangeHPServerRpc(damage, targetClientId, ownerPlayerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(float damage, ulong targetClientId, ulong ownerClientId)
    {
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        var ownerPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerRoot>(out var targetPlayerRoot))
        {
            if (targetPlayerRoot.PlayerTakeDamage.HP.Value == 0) return;

            targetPlayerRoot.PlayerTakeDamage.HP.Value -= damage;
            if (targetPlayerRoot.PlayerTakeDamage.HP.Value <= 0)
            {
                targetPlayerRoot.PlayerNetwork.DeathCount.Value += 1;
                if (ownerPlayer.TryGetComponent<PlayerNetwork>(out var ownerPlayerNetwork))
                {
                    ownerPlayerNetwork.KillCount.Value += 1;
                }
                targetPlayerRoot.PlayerTakeDamage.HP.Value = 0;
                InGameManager.Instance.KillCountChecker.CheckPlayerKillCount(ownerPlayerNetwork.KillCount.Value);
            }

            Debug.Log($"{targetClientId} current HP: {targetPlayerRoot.PlayerTakeDamage.HP.Value}");
        }

        PlayerRoot.PlayerUI.AddTakeDamageEffect(damage, targetClientId);
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