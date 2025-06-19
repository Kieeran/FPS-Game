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

    public Action ToggleEscapeUI;

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
        if (!IsOwner) return;
        CurrentPlayerCanvas = Instantiate(_playerCanvas, transform);

        CurrentPlayerCanvas.EscapeUI.OnQuitGame += QuitGame;

        PlayerRoot.PlayerAim.OnAim += () =>
        {
            CurrentPlayerCanvas.ToggleCrossHair(false);
        };

        PlayerRoot.PlayerAim.OnUnAim += () =>
        {
            CurrentPlayerCanvas.ToggleCrossHair(true);
        };

        InGameManager.Instance.TimePhaseCounter.OnTimeChanged += UpdateTimerUI;

        InGameManager.Instance.KillCountChecker.OnGameEnd += () =>
        {
            if (PlayerRoot.PlayerNetwork.KillCount.Value >= InGameManager.Instance.KillCountChecker.MaxKillCount)
                CurrentPlayerCanvas.PopUpVictoryDefeat("VICTORY");
            else
                CurrentPlayerCanvas.PopUpVictoryDefeat("DEFEAT");

            PlayerRoot.PlayerAssetsInputs.IsInputEnabled = false;

            CurrentPlayerCanvas.PlayEndGameFadeOut(() =>
            {
                QuitGame();
            });
        };
    }

    public void AddTakeDamageEffect(float damage, ulong targetClientId)
    {
        AddTakeDamageEffect_ServerRpc(damage, targetClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddTakeDamageEffect_ServerRpc(float damage, ulong targetClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { targetClientId }
            }
        };

        AddTakeDamageEffect_ClientRpc(damage, clientRpcParams);
    }

    [ClientRpc]
    public void AddTakeDamageEffect_ClientRpc(float damage, ClientRpcParams clientRpcParams)
    {
        CurrentPlayerCanvas.HitEffect.StartFadeHitEffect(damage);
    }

    void QuitGame()
    {
        // Gửi sự kiện cho tất cả Client để xử lý thoát game
        NotifyClientsToQuit_ServerRpc();

        NetworkManager.Singleton.Shutdown();
        LobbyManager.Instance.ExitGame();

        // GameSceneManager.Instance.LoadPreviousScene();
        GameSceneManager.Instance.LoadScene("Lobby Room");
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

    void UpdateTimerUI(int seconds)
    {
        int mins = seconds / 60;
        int secs = seconds % 60;
        CurrentPlayerCanvas.UpdateTimerNum(mins, secs);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.escapeUI == true)
        {
            ToggleEscapeUI?.Invoke();

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