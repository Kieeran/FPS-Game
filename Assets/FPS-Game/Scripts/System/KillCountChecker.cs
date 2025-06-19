using System;
using System.Data;
using UnityEngine;

public class KillCountChecker : MonoBehaviour
{
    public int MaxKillCount;
    public Action OnGameEnd;

    bool _isGameEnd = false;

    public void CheckPlayerKillCount(int killCount)
    {
        if (_isGameEnd) return;
        if (killCount >= MaxKillCount)
        {
            Debug.Log("Game End");
            OnGameEnd?.Invoke();
            _isGameEnd = true;
        }

        else
        {
            Debug.Log($"MaxKillCount: {MaxKillCount}, current MaxKillCount: {killCount}");
        }
    }
}
