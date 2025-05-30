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
    public float waitingPhaseDuration;
    public float preparationPhaseDuration;
    public float combatPhaseDuration;
    public float resultPhaseDuration;

    [HideInInspector]
    public NetworkVariable<float> CurrentPhaseTimeRemaining = new(0f);
    NetworkVariable<MatchPhase> currentPhase = new(MatchPhase.Waiting);

    bool _isPhaseRunning = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartPhase(MatchPhase.Waiting, waitingPhaseDuration);
        }

        currentPhase.OnValueChanged += OnPhaseChanged;
        CurrentPhaseTimeRemaining.OnValueChanged += (oldVal, newVal) => UpdateTimerUI(newVal);
    }

    void Update()
    {
        if (!IsServer || !_isPhaseRunning) return;

        if (CurrentPhaseTimeRemaining.Value > 0)
        {
            CurrentPhaseTimeRemaining.Value -= Time.deltaTime;
        }
        else
        {
            CurrentPhaseTimeRemaining.Value = 0;
            _isPhaseRunning = false;
            OnPhaseEnded();
        }
    }

    private void OnPhaseEnded()
    {
        switch (currentPhase.Value)
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
                Debug.Log("Match Ended!");
                // TODO: Kết thúc game hoặc quay về lobby
                break;
        }
    }

    void OnPhaseChanged(MatchPhase oldPhase, MatchPhase newPhase)
    {
        // Apply text (visualize phase number)
    }

    void StartPhase(MatchPhase phase, float duration)
    {
        currentPhase.Value = phase;
        CurrentPhaseTimeRemaining.Value = duration;
        _isPhaseRunning = true;
    }

    void UpdateTimerUI(float seconds)
    {
        //Apply timer number text
    }
}