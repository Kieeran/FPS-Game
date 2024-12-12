using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTakeDamage : NetworkBehaviour
{
    private NetworkVariable<float> HP = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Image health;
    [SerializeField] private Image hitEffect;

    public float hitBodyAlpha;
    public float hitHeadAlpha;

    public void TakeDamage(float damage, ulong targetClientId)
    {
        //Debug.Log($"{OwnerClientId} take {damage} damage");
        ChangeHPServerRpc(damage, targetClientId);

        UpdateUI(damage, targetClientId);
    }

    private void Update()
    {
        if (!IsOwner) return;

        health.fillAmount = HP.Value;

        if (hitEffect.color.a != 0)
        {
            hitEffect.color = new Color(1, 0, 0, hitEffect.color.a - Time.deltaTime / 1.5f);
            if (hitEffect.color.a <= 0)
            {
                hitEffect.color = new Color(1, 0, 0, 0);
            }
        }
    }

    public void UpdateUI(float damage, ulong targetClientId)
    {
        if (damage == 0.05f)
            UpdateUIServerRpc(hitBodyAlpha / 255, targetClientId);
        else if (damage == 0.1f)
            UpdateUIServerRpc(hitHeadAlpha / 255, targetClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(float damage, ulong targetClientId)
    {
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var targetHealth))
        {
            targetHealth.HP.Value -= damage;

            if (targetHealth.HP.Value <= 0) targetHealth.HP.Value = 1;

            //Debug.Log($"{targetClientId} current HP: {targetHealth.HP.Value}");
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
        hitEffect.color = new Color(1, 0, 0, alpha);
    }

    // private IEnumerator FadeHitEffect(Image hitEffect, float targetAlpha, ulong targetClientId)
    // {
    //     if (OwnerClientId == targetClientId)
    //     {
    //         float currentAlpha = targetAlpha;

    //         // if (!IsOwner) currentAlpha = 0;

    //         float fadeTime = 50f; // Adjust fade time as needed

    //         while (currentAlpha > targetAlpha)
    //         {
    //             hitEffect.color = new Color(1, 0, 0, currentAlpha);
    //             currentAlpha -= Time.deltaTime / fadeTime;
    //             yield return null;
    //         }
    //     }
    // }
}