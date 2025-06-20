using System;
using Unity.Netcode;
using UnityEngine;

public class KillCountChecker : NetworkBehaviour
{
    public int MaxKillCount;
    public float EndGameSlowFactor;

    bool _isGameEnd = false;

    public void CheckPlayerKillCount(int killCount)
    {
        if (_isGameEnd) return;
        if (killCount >= MaxKillCount)
        {
            NotifyGameEnd_ServerRpc();
            Debug.Log("Game End");
            _isGameEnd = true;
        }

        else
        {
            Debug.Log($"MaxKillCount: {MaxKillCount}, current MaxKillCount: {killCount}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyGameEnd_ServerRpc()
    {
        NotifyGameEnd_ClientRpc();
    }

    [ClientRpc]
    void NotifyGameEnd_ClientRpc()
    {
        InGameManager.Instance.OnGameEnd?.Invoke();
        Time.timeScale = EndGameSlowFactor;
    }
}
