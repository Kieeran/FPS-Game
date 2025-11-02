using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerTakeDamage : PlayerBehaviour
{
    public NetworkVariable<float> HP = new(1);
    public bool IsPlayerDead() { return HP.Value <= 0; }

    // OnNetworkSpawn
    public override int PriorityNetwork => 15;
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        HP.OnValueChanged += OnHPChanged;
        PlayerRoot.Events.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnHPChanged(float previous, float current)
    {
        if (previous == current) return;

        if (IsOwner && !PlayerRoot.IsCharacterBot())
            PlayerRoot.PlayerUI.CurrentPlayerCanvas.HealthBar.UpdatePlayerHealthBar(current);

        if (current == 0)
        {
            PlayerRoot.Events.InvokeOnPlayerDead();
            InGameManager.Instance.GenerateHealthPickup.DropHealthPickup(transform.position);
        }
    }

    void OnPlayerRespawn()
    {
        ResetPlayerHP_ServerRpc(OwnerClientId);
    }

    public void TakeDamage(float damage, ulong targetClientId, ulong ownerPlayerID, bool forBot = false)
    {
        if (forBot)
        {
            ChangeHPForBot_ServerRpc(damage, targetClientId, ownerPlayerID);
        }
        else
        {
            ChangeHPServerRpc(damage, targetClientId, ownerPlayerID);
        }
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
    public void ChangeHPForBot_ServerRpc(float damage, ulong targetID, ulong ownerId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetID, out var botObj))
        {
            Debug.Log($"Tìm thấy object: {botObj.name}");
            if (botObj.TryGetComponent<PlayerRoot>(out var botRoot))
            {
                if (botRoot.PlayerTakeDamage.HP.Value == 0) return;

                botRoot.PlayerTakeDamage.HP.Value -= damage;
                if (botRoot.PlayerTakeDamage.HP.Value < 0)
                {
                    botRoot.PlayerNetwork.DeathCount.Value += 1;
                    botRoot.PlayerTakeDamage.HP.Value = 0;
                }
            }
        }
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