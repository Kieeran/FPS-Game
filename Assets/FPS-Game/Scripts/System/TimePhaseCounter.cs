using System;
using Unity.Netcode;
using UnityEngine;

public enum MatchPhase
{
    Waiting,
    Preparation,
    Combat,
    Result
}

public class TimePhaseCounter : NetworkBehaviour
{
    [Header("Network Variable")]
    NetworkVariable<MatchPhase> _currentPhase = new(MatchPhase.Waiting);
    NetworkVariable<double> _currentPhaseStartTime = new(0);
    NetworkVariable<float> _currentPhaseDuration = new(0);
    NetworkVariable<bool> _countdownStarted = new(false);

    [Header("Phase Durations")]
    public float waitingPhaseDuration;
    public float preparationPhaseDuration;
    public float combatPhaseDuration;
    public float resultPhaseDuration;

    [Space(10)]
    int _lastDisplayedSeconds = -1;

    public Action<int> OnTimeChanged;

    [SerializeField] LobbyRelayChecker _lobbyRelayChecker;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartPhase(MatchPhase.Waiting, waitingPhaseDuration);

            _lobbyRelayChecker.StartChecking(LobbyManager.joinedLobby.Id);
            _lobbyRelayChecker.onAllPlayersConnected += () =>
            {
                _countdownStarted.Value = true;
            };
        }

        // UIs
        _currentPhase.OnValueChanged += OnPhaseChanged;
    }

    void Update()
    {
        if (!_countdownStarted.Value) return;

        double timeElapsed = NetworkManager.Singleton.LocalTime.Time - _currentPhaseStartTime.Value;
        float remaining = Mathf.Max(_currentPhaseDuration.Value - (float)timeElapsed, 0f);

        UpdateTimerUI(remaining);

        int secondsToDisplay = Mathf.FloorToInt(remaining);
        if (secondsToDisplay != _lastDisplayedSeconds)
        {
            _lastDisplayedSeconds = secondsToDisplay;
            OnTimeChanged?.Invoke(secondsToDisplay);
        }

        if (IsServer && remaining <= 0)
        {
            AdvancePhase();
        }
    }

    private void AdvancePhase()
    {
        switch (_currentPhase.Value)
        {
            case MatchPhase.Waiting:
                StartPhase(MatchPhase.Preparation, preparationPhaseDuration);
                break;
            case MatchPhase.Preparation:
                StartPhase(MatchPhase.Combat, combatPhaseDuration);
                break;
            case MatchPhase.Combat:
                StartPhase(MatchPhase.Result, resultPhaseDuration);
                break;
            case MatchPhase.Result:
                Debug.Log("Match ended.");
                // TODO: Gọi hàm kết thúc trận đấu hoặc  quay về lobby
                break;
        }
    }

    void OnPhaseChanged(MatchPhase oldPhase, MatchPhase newPhase)
    {
        // Apply text (visualize phase number)
    }

    void StartPhase(MatchPhase phase, float duration)
    {
        _currentPhase.Value = phase;

        _currentPhaseStartTime.Value = NetworkManager.ServerTime.Time;
        _currentPhaseDuration.Value = duration;
    }

    void UpdateTimerUI(float seconds)
    {
        //Apply timer number text
    }
}