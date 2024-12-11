using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTakeDamage : NetworkBehaviour
{
    private NetworkVariable<float> HP = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // private NetworkVariable<bool> startHitEffect = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> alpha = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Image health;
    [SerializeField] private Image hitEffect;

    public float hitBodyAlpha;
    public float hitHeadAlpha;

    public void TakeDamage(float damage, ulong targetClientId)
    {
        Debug.Log($"{OwnerClientId} take {damage} damage");
        ChangeHPServerRpc(damage, targetClientId);

        UpdateUI(damage, targetClientId);
    }

    private void Update()
    {
        if (!IsOwner) return;

        health.fillAmount = HP.Value;

        // if (startHitEffect.Value == true)
        if (hitEffect.color.a != 0)
        {
            //hitEffect.color = new Color(1, 0, 0, hitEffect.color.a - Time.deltaTime * alpha.Value / 255);
            hitEffect.color = new Color(1, 0, 0, hitEffect.color.a - Time.deltaTime * 5);
            if (hitEffect.color.a <= 0)
            {
                // startHitEffect = false;
                //hitEffect.gameObject.SetActive(false);

                hitEffect.color = new Color(1, 0, 0, 0);
            }
        }
    }

    public void UpdateUI(float damage, ulong targetClientId)
    {
        if (damage == 0.05f)
            ChangeHPServerRpc(hitBodyAlpha / 255, targetClientId);
        else if (damage == 0.1f)
            ChangeHPServerRpc(hitHeadAlpha / 255, targetClientId);

        hitEffect.color = new Color(1, 0, 0, alpha.Value);
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

    [ServerRpc(RequireOwnership = false)]
    public void UpdateUIServerRpc(float alpha, ulong targetClientId)
    {
        // Tìm đối tượng của client mục tiêu
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        if (targetPlayer.TryGetComponent<PlayerTakeDamage>(out var player))
        {
            //player.startHitEffect.Value = true;
            player.alpha.Value = alpha;
        }
    }
}