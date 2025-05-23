using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public PlayerCanvas CurrentPlayerCanvas { get; private set; }

    [SerializeField] PlayerCanvas _playerCanvas;

    ReloadEffect _reloadEffect;

    public Action OnOpenScoreBoard;

    void Awake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
        CurrentPlayerCanvas = Instantiate(_playerCanvas, transform);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        CurrentPlayerCanvas.EscapeUI.OnQuitGame += (() =>
        {
            if (IsOwner == false) return;

            // Gửi sự kiện cho tất cả Client để xử lý thoát game
            NotifyClientsToQuit_ServerRpc();

            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.ExitGame();

            // GameSceneManager.Instance.LoadPreviousScene();
            GameSceneManager.Instance.LoadScene("Lobby Room");
        });

        PlayerRoot.PlayerAim.OnAim += () =>
        {
            CurrentPlayerCanvas.ToggleCrossHair(false);
        };

        PlayerRoot.PlayerAim.OnUnAim += () =>
        {
            CurrentPlayerCanvas.ToggleCrossHair(true);
        };
    }

    public void UpdateUI(float damage, ulong targetClientId)
    {
        UpdateUIServerRpc(damage, targetClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateUIServerRpc(float damage, ulong targetClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { targetClientId }
            }
        };

        UpdateUIClientRpc(damage, clientRpcParams);
    }

    [ClientRpc]
    public void UpdateUIClientRpc(float damage, ClientRpcParams clientRpcParams)
    {
        CurrentPlayerCanvas.HitEffect.StartFadeHitEffect(damage);
    }

    [ServerRpc]
    private void NotifyClientsToQuit_ServerRpc()
    {
        NotifyClientsToQuit_ClientRpc();
    }

    [ClientRpc]
    private void NotifyClientsToQuit_ClientRpc()
    {
        // Hành động cho từng Client khi host thoát
        if (!IsOwner)
        {
            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.ExitGame();

            // GameSceneManager.Instance.LoadPreviousScene();
            GameSceneManager.Instance.LoadScene("Lobby Room");
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.escapeUI == true)
        {
            CurrentPlayerCanvas.ToggleEscapeUI();
            PlayerRoot.PlayerAssetsInputs.escapeUI = false;
        }

        if (PlayerRoot.PlayerAssetsInputs.openScoreboard == true)
        {
            CurrentPlayerCanvas.ToggleScoreBoard();
            PlayerRoot.PlayerAssetsInputs.openScoreboard = false;
        }
    }
}