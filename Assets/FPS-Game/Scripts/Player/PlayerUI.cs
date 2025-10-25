using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : PlayerBehaviour
{
    public PlayerCanvas CurrentPlayerCanvas { get; private set; }

    [SerializeField] PlayerCanvas _playerCanvas;

    bool _toggleEscapeUI = false;

    public bool IsEscapeUIOn()
    {
        return _toggleEscapeUI;
    }

    // OnNetworkSpawn
    public override int PriorityNetwork => 10;
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        if (!IsOwner) return;
        if (PlayerRoot.IsBot.Value) return;
        CurrentPlayerCanvas = Instantiate(_playerCanvas);

        PlayerRoot.Events.OnQuitGame += QuitGame;

        PlayerRoot.Events.OnAimStateChanged += (isAim) =>
        {
            if (isAim)
            {
                CurrentPlayerCanvas.ToggleCrossHair(false);
            }

            else
            {
                GameObject weapon = PlayerRoot.WeaponHolder.GetCurrentWeapon();
                if (weapon.TryGetComponent<Gun>(out var currentGun))
                {
                    CurrentPlayerCanvas.ToggleCrossHair(true);
                }
            }
        };

        InGameManager.Instance.TimePhaseCounter.OnTimeChanged += UpdateTimerUI;
        InGameManager.Instance.OnReceivedPlayerInfo += (playerInfos) =>
        {
            int currentMaxKillCountIndex = 0;
            for (int i = 1; i < playerInfos.Count; i++)
            {
                if (playerInfos[i].KillCount > playerInfos[currentMaxKillCountIndex].KillCount)
                {
                    currentMaxKillCountIndex = i;
                }
            }

            if (!InGameManager.Instance.IsTimeOut.Value &&
            playerInfos[currentMaxKillCountIndex].KillCount < InGameManager.Instance.KillCountChecker.MaxKillCount)
                return;

            if (playerInfos[currentMaxKillCountIndex].PlayerId == OwnerClientId)
                CurrentPlayerCanvas.PopUpVictoryDefeat("VICTORY");
            else
                CurrentPlayerCanvas.PopUpVictoryDefeat("DEFEAT");
        };
        InGameManager.Instance.OnGameEnd += () =>
        {
            InGameManager.Instance.GetAllPlayerInfos();
            PlayerRoot.PlayerAssetsInputs.IsInputEnabled = false;
            CurrentPlayerCanvas.PlayEndGameFadeOut(() =>
            {
                QuitGame();
            });
        };

        PlayerRoot.Events.OnCollectedHealthPickup += () =>
        {
            CurrentPlayerCanvas.HealRefillAmmoEffect.StartEffect();
        };

        PlayerRoot.Events.OnWeaponChanged += (sender, e) =>
        {
            if (e.CurrentWeapon.TryGetComponent<Gun>(out var currentGun))
            {
                CurrentPlayerCanvas.ToggleCrossHair(true);
            }

            else
            {
                CurrentPlayerCanvas.ToggleCrossHair(false);
            }
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
            _toggleEscapeUI = !_toggleEscapeUI;
            PlayerRoot.Events.InvokeToggleEscapeUI();

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