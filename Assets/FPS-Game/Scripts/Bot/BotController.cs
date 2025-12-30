using System;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

public enum State
{
    None,
    Idle,
    Patrol,
    Combat
}

namespace AIBot
{
    /// <summary>
    /// Central FSM owner for a demo bot.
    /// Controls a minimal state machine (Idle -> Patrol -> Combat),
    /// and activates the corresponding Behavior Designer Behavior for each state.
    ///
    /// Important: Behavior Designer's Behavior component does NOT expose IsRunning/isPaused,
    /// so this controller tracks the currently active Behavior instance and uses
    /// StartBehavior/StopBehavior + enabled to control it.
    /// </summary>
    [DisallowMultipleComponent]
    public class BotController : MonoBehaviour
    {
        [Header("Behavior Designer Behaviors")]
        [SerializeField] Behavior idleBehavior;
        [SerializeField] Behavior patrolBehavior;
        [SerializeField] Behavior combatBehavior;

        [Header("References")]
        [Tooltip("Sensor used to detect the player.")]
        [SerializeField] PerceptionSensor sensor;

        [Tooltip("Adapter that synchronizes C# blackboard values to BD SharedVariables")]
        [SerializeField] BlackboardLinker blackboardLinker;

        // [Tooltip("Seconds allowed without seeing player before returning to patrol")]
        // public float lostSightTimeout = 2f;

        // runtime state
        State currentState = State.None;
        // private float _lostSightStart = -1f;

        // explicit currently active Behavior component (null when none)
        private Behavior _activeBehavior;
        public PlayerRoot PlayerRoot { get; private set; }

        void Awake()
        {
            PlayerRoot = transform.root.GetComponent<PlayerRoot>();

            GlobalVariables.Instance.SetVariable("botCamera", new SharedTransform()
            {
                Value = PlayerRoot.PlayerCamera.GetPlayerCameraTarget()
            });
        }

        void Start()
        {
            InitController();
        }

        private void Update()
        {
            UpdateValues();
        }

        void UpdateValues()
        {
            switch (currentState)
            {
                case State.Idle:
                    PlayerRoot.AIInputFeeder.OnLook?.Invoke(blackboardLinker.GetLookEuler());
                    break;

                case State.Patrol:
                    PlayerRoot.AIInputFeeder.OnMove?.Invoke(blackboardLinker.GetMovDir());
                    break;

                case State.Combat:
                    PlayerRoot.AIInputFeeder.OnLook?.Invoke(blackboardLinker.GetLookEuler());
                    PlayerRoot.AIInputFeeder.OnAttack?.Invoke(blackboardLinker.GetAttack());
                    PlayerRoot.AIInputFeeder.OnMove?.Invoke(blackboardLinker.GetMovDir());

                    blackboardLinker.SetLastKnownPlayerData(sensor.GetLastKnownPlayerData());
                    // // If player currently not visible, start lost sight timer; otherwise reset
                    // if (!blackboardLinker?.isPlayerVisible ?? true)
                    // {
                    //     if (_lostSightStart < 0f) _lostSightStart = Time.time;
                    //     else if (Time.time - _lostSightStart >= lostSightTimeout)
                    //     {
                    //         // Timed out -> go back to patrol
                    //         SwitchToState(FSMState.CurrentState.Patrol);
                    //     }
                    // }
                    // else
                    // {
                    //     _lostSightStart = -1f;
                    // }
                    break;
            }

            blackboardLinker.SetTargetPlayer(sensor.GetTargetPlayerTransform());
        }

        void InitController()
        {
            PlayerRoot.AIInputFeeder.enabled = true;
            // Basic validation
            if (blackboardLinker == null) Debug.LogWarning("[BotController] BlackboardLinker not assigned.");
            if (sensor == null) Debug.LogWarning("[BotController] PerceptionSensor not assigned.");
            // if (waypointPath == null) Debug.LogWarning("[BotController] WaypointPath not assigned.");

            // Subscribe perception events (safe if perception is null)
            // if (perception != null)
            // {
            //     perception.OnPlayerSpotted += HandlePlayerSpotted;
            //     perception.OnPlayerLost += HandlePlayerLost;
            // }

            SwitchToState(State.Idle);
        }

        // private void OnDestroy()
        // {
        //     if (perception != null)
        //     {
        //         perception.OnPlayerSpotted -= HandlePlayerSpotted;
        //         perception.OnPlayerLost -= HandlePlayerLost;
        //     }

        //     // Ensure we stop any active behavior cleanly
        //     if (_activeBehavior != null)
        //     {
        //         StopBehavior(_activeBehavior);
        //         _activeBehavior = null;
        //     }
        // }

        public void OnSwitchState(string state)
        {
            switch (state)
            {
                case "Idle":
                    SwitchToState(State.Idle);
                    break;
                case "Patrol":
                    SwitchToState(State.Patrol);
                    break;
                case "Combat":
                    SwitchToState(State.Combat);
                    break;
            }

            PlayerRoot.AIInputFeeder.OnMove?.Invoke(Vector3.zero);
            PlayerRoot.AIInputFeeder.OnLook?.Invoke(Vector3.zero);
        }

        /// <summary>
        /// Switches the FSM to a new state and activates the corresponding Behavior Designer Behavior.
        /// </summary>
        /// <param name="newState">Target state</param>
        public void SwitchToState(State newState)
        {
            if (currentState == newState) return;

            // stop any previously active Behavior
            if (_activeBehavior != null)
            {
                StopBehavior(_activeBehavior);
                _activeBehavior = null;
            }

            currentState = newState;
            // _lostSightStart = -1f;

            // choose and start the matching Behavior
            switch (newState)
            {
                case State.Idle:
                    Debug.Log("Entering Idle State");
                    StartBehavior(idleBehavior);
                    break;
                case State.Patrol:
                    Debug.Log("Entering Patrol State");
                    StartBehavior(patrolBehavior);
                    break;
                case State.Combat:
                    Debug.Log("Entering Combat State");
                    StartBehavior(combatBehavior);
                    break;
            }

            Debug.Log($"[BotController] Switched to {currentState}");
        }

        /// <summary>
        /// Safely starts a Behavior component and tells the BlackboardLinker to bind to it.
        /// </summary>
        /// <param name="b">Behavior to start (may be null)</param>
        private void StartBehavior(Behavior b)
        {
            if (b == null)
            {
                Debug.Log("Current Behaviour is null");
                return;
            }

            try
            {
                // Enable component (so BD lifecycle occurs)
                if (!b.enabled) b.enabled = true;

                // Start the behavior explicitly (safe even if already running)
                b.EnableBehavior();

                // Track active behavior
                _activeBehavior = b;

                // Seed BD SharedVariables from our C# blackboard adapter
                blackboardLinker?.BindToBehavior(b);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BotController] Failed to start Behavior: {ex.Message}");
            }
        }

        /// <summary>
        /// Safely stops a Behavior component.
        /// </summary>
        /// <param name="b">Behavior to stop (may be null)</param>
        private void StopBehavior(Behavior b)
        {
            if (b == null) return;

            try
            {
                // Ask the behavior to stop
                b.DisableBehavior();

                // Optionally disable component to prevent accidental autostart
                if (b.enabled) b.enabled = false;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BotController] Failed to stop Behavior: {ex.Message}");
            }
        }

        #region Perception Event Handlers

        // private void HandlePlayerSpotted(Vector3 lastSeenWorldPos, GameObject playerGameObject)
        // {
        //     // update blackboard via linker
        //     blackboardLinker?.SetPlayerVisible(true, lastSeenWorldPos, playerGameObject);

        //     // cancel lost-sight timer if any
        //     _lostSightStart = -1f;

        //     // immediately transition to combat (FSM is authoritative)
        //     SwitchToState(FSMState.CurrentState.Combat);
        // }

        // private void HandlePlayerLost()
        // {
        //     // Mark not visible; BotController's Update will start/handle the timeout when in Combat
        //     blackboardLinker?.SetPlayerVisible(false, blackboardLinker?.PlayerLastSeenPos ?? Vector3.zero, null);

        //     // Start the lost-sight timer if currently in combat
        //     if (_state == FSMState.CurrentState.Combat && _lostSightStart < 0f)
        //         _lostSightStart = Time.time;
        // }

        #endregion
    }
}
