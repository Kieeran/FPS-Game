using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public Image Effect { get; private set; }
    [SerializeField] float _hitBodyAlpha;
    [SerializeField] float _hitHeadAlpha;

    void Awake()
    {
        Effect = GetComponent<Image>();
    }

    public void UpdateUI(float damage, ulong targetClientId)
    {
        if (damage == 0.05f)
        {
            UpdateUIServerRpc(_hitBodyAlpha / 255, targetClientId);
        }
        else if (damage == 0.1f)
        {
            UpdateUIServerRpc(_hitHeadAlpha / 255, targetClientId);
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
        StartCoroutine(FadeHitEffect(Effect, alpha));
    }

    private IEnumerator FadeHitEffect(Image hitEffect, float targetAlpha)
    {
        float currentAlpha = targetAlpha;

        while (currentAlpha > 0)
        {
            hitEffect.color = new Color(1, 0, 0, currentAlpha);
            currentAlpha -= Time.deltaTime;
            yield return null;
        }
    }
}
