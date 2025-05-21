using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerTakeDamage : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<float> HP = new(1);
    [SerializeField] private Image health;
    [SerializeField] private Image hitEffect;

    public float hitBodyAlpha;
    public float hitHeadAlpha;

    public Action PlayerDead;
    public bool IsPlayerDead = false;

    public override void OnNetworkSpawn()
    {
        HP.OnValueChanged += OnHPChanged;
    }

    private void OnHPChanged(float previous, float current)
    {
        if (previous == current) return;

        health.fillAmount = current;

        if (previous == 0) IsPlayerDead = false;

        if (current == 0)
        {
            IsPlayerDead = true;
            PlayerDead?.Invoke();
        }
    }

    public void TakeDamage(float damage, ulong targetClientId, ulong ownerPlayerID)
    {
        ChangeHPServerRpc(damage, targetClientId, ownerPlayerID);
    }

    public void UpdateUI(float damage, ulong targetClientId)
    {
        if (damage == 0.05f)
        {
            UpdateUIServerRpc(hitBodyAlpha / 255, targetClientId);
        }
        else if (damage == 0.1f)
        {
            UpdateUIServerRpc(hitHeadAlpha / 255, targetClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(float damage, ulong targetClientId, ulong ownerClientId)
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
                targetHealth.HP.Value = 0;
            }

            Debug.Log($"{targetClientId} current HP: {targetHealth.HP.Value}");
        }

        UpdateUI(damage, targetClientId);
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

    [ServerRpc(RequireOwnership = false)]
    public void UpdateUIServerRpc(float alpha, ulong targetClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { targetClientId }
            }
        };

        UpdateUIClientRpc(alpha, clientRpcParams);
    }

    [ClientRpc]
    public void UpdateUIClientRpc(float alpha, ClientRpcParams clientRpcParams)
    {
        //Debug.Log($"{OwnerClientId} start hit UI with alpha: {alpha}");
        //hitEffect.color = new Color(1, 0, 0, alpha);
        StartCoroutine(FadeHitEffect(hitEffect, alpha));
    }

    private IEnumerator FadeHitEffect(Image hitEffect, float targetAlpha)
    {
        float currentAlpha = targetAlpha;

        //Debug.Log($"{OwnerClientId} has alpha: {targetAlpha}");

        while (currentAlpha > 0)
        {
            // Debug.Log(OwnerClientId);

            hitEffect.color = new Color(1, 0, 0, currentAlpha);
            currentAlpha -= Time.deltaTime;
            yield return null;
        }
    }
}