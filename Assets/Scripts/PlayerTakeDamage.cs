using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using UnityEngine.InputSystem;
using PlayerInfoNameSpace;

public class PlayerTakeDamage : NetworkBehaviour
{
    private NetworkVariable<float> HP = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // public static NetworkVariable<int> killCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // public static NetworkVariable<int> deathCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Image health;
    [SerializeField] private Image hitEffect;

    public PlayerInfo playerInfo;

    public float hitBodyAlpha;
    public float hitHeadAlpha;

    public void TakeDamage(float damage, ulong targetClientId)
    {
        // Debug.Log($"{OwnerClientId} take {damage} damage");
        ChangeHPServerRpc(damage, targetClientId);

        UpdateUI(damage, targetClientId);
    }

    private void Update()
    {
        if (!IsOwner) return;

        //health.fillAmount = HP.Value;
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

    public static float GetPlayerHP(ulong targetClientId)
    {
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            return targetHealth.HP.Value;
        }
        else return 69;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(float damage, ulong targetClientId)
    {
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            targetHealth.HP.Value -= damage;

            if (targetHealth.HP.Value <= 0 && playerInfo.ClientId == targetClientId)
            {
                // playerInfo.ClientId = targetClientId;
                PlayerInfo.deathCount.Value += 1;
                targetHealth.HP.Value = 1;
            }

            if (targetHealth.HP.Value <= 0 && playerInfo.ClientId != targetClientId)
            {
                PlayerInfo.killCount.Value += 1;
                targetHealth.HP.Value = 1;
            }
            
            Debug.Log($"{targetClientId} current HP: {targetHealth.HP.Value}");
        }

        ChangeHPClientRpc(
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { targetClientId }
                }
            }
        );
    }

    [ClientRpc]
    public void ChangeHPClientRpc(ClientRpcParams clientRpcParams)
    {
        // var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        // if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        // {
        //     targetHealth.HP.Value -= damage;

        //     if (targetHealth.HP.Value <= 0) targetHealth.HP.Value = 1;

        //     //Debug.Log($"{targetClientId} current HP: {targetHealth.HP.Value}");
        // }

        health.fillAmount = HP.Value;
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

    // public void UpdateDeathCountServerRpc(int deathCount, ulong targetClientId)
    // {
    //     ClientRpcParams clientRpcParams = new ClientRpcParams
    //     {
    //         Send = new ClientRpcSendParams
    //         {
    //             TargetClientIds = new List<ulong> {targetClientId}
    //         }
    //     };

    //     UpdateDeathCountClientRpc(deathCount, clientRpcParams);
    // }

    [ClientRpc]
    public void UpdateUIClientRpc(float alpha, ClientRpcParams clientRpcParams)
    {
        //Debug.Log($"{OwnerClientId} start hit UI with alpha: {alpha}");
        //hitEffect.color = new Color(1, 0, 0, alpha);
        StartCoroutine(FadeHitEffect(hitEffect, alpha));
    }

    // public void UpdateDeathCountClientRpc(int deathCount, ClientRpcParams clientRpcParams)
    // {
    //     if (HP)
    // }

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