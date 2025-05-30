using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour, IInitAwake, IInitNetwork
{
    public PlayerRoot PlayerRoot { get; private set; }
    public PlayerCanvas CurrentPlayerCanvas { get; private set; }

    [SerializeField] PlayerCanvas _playerCanvas;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    // OnNetworkSpawn
    public int PriorityNetwork => 10;
    public void InitializeOnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner == false) return;
        CurrentPlayerCanvas = Instantiate(_playerCanvas, transform);

        CurrentPlayerCanvas.EscapeUI.OnQuitGame += (() =>
        {
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

    public void AddTakeDamageEffect(string shotType, ulong targetClientId)
    {
        AddTakeDamageEffect_ServerRpc(shotType, targetClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddTakeDamageEffect_ServerRpc(string shotType, ulong targetClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { targetClientId }
            }
        };

        AddTakeDamageEffect_ClientRpc(shotType, clientRpcParams);
    }

    [ClientRpc]
    public void AddTakeDamageEffect_ClientRpc(string shotType, ClientRpcParams clientRpcParams)
    {
        CurrentPlayerCanvas.HitEffect.StartFadeHitEffect(shotType);
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