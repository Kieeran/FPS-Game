using System;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

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
        [Header("Startup")]
        [Tooltip("Initial runtime state.")]
        public FSMState.InitialState startState = FSMState.InitialState.Idle;

        [Header("Behavior Designer Behaviors (assign one Behavior component per tree)")]
        [Tooltip("Behavior component with Idle tree")]
        public Behavior idleBehavior;

        [Tooltip("Behavior component with Patrol tree")]
        public Behavior patrolBehavior;

        // [Tooltip("Behavior component with Combat tree")]
        // public Behavior combatBehavior;

        // [Header("References")]
        // [Tooltip("Sensor that raises OnPlayerSpotted / OnPlayerLost")]
        // public PerceptionSensor perception;

        [Tooltip("Adapter that synchronizes C# blackboard values to BD SharedVariables")]
        public BlackboardLinker blackboardLinker;

        [Tooltip("Waypoint holder for Patrol")]
        public WaypointPath waypointPath;

        [Header("Parameters")]
        [Tooltip("Seconds to remain idle before starting patrol")]
        public float idleDuration = 4f;

        // [Tooltip("Seconds allowed without seeing player before returning to patrol")]
        // public float lostSightTimeout = 2f;

        // runtime state
        private FSMState.CurrentState _state = FSMState.CurrentState.None;
        private float _stateEnterTime;
        // private float _lostSightStart = -1f;

        // explicit currently active Behavior component (null when none)
        private Behavior _activeBehavior;
        public PlayerRoot PlayerRoot { get; private set; }

        void Awake()
        {
            PlayerRoot = transform.root.GetComponent<PlayerRoot>();
        }

        // private void Reset()
        // {
        //     // Try to auto-wire common components if left unassigned in inspector
        //     perception ??= GetComponent<PerceptionSensor>();
        //     blackboardLinker ??= GetComponent<BlackboardLinker>();
        //     waypointPath ??= GetComponent<WaypointPath>();
        // }

        private void Start()
        {
            PlayerRoot.AIInputFeeder.enabled = true;
            // Basic validation
            if (blackboardLinker == null) Debug.LogWarning("[BotController] BlackboardLinker not assigned.");
            // if (perception == null) Debug.LogWarning("[BotController] PerceptionSensor not assigned.");
            if (waypointPath == null) Debug.LogWarning("[BotController] WaypointPath not assigned.");

            // Subscribe perception events (safe if perception is null)
            // if (perception != null)
            // {
            //     perception.OnPlayerSpotted += HandlePlayerSpotted;
            //     perception.OnPlayerLost += HandlePlayerLost;
            // }

            // initialize FSM to configured start state
            var initial = startState == FSMState.InitialState.Idle ? FSMState.CurrentState.Idle
                        : (startState == FSMState.InitialState.Patrol ? FSMState.CurrentState.Patrol
                        : FSMState.CurrentState.Combat);

            SwitchToState(initial);
        }

        private void Update()
        {
            // handle per-state timing logic
            switch (_state)
            {
                case FSMState.CurrentState.Idle:
                    if (IsIdleTimeout())
                    {
                        SwitchToState(FSMState.CurrentState.Patrol);
                    }
                    break;

                case FSMState.CurrentState.Patrol:
                    PlayerRoot.AIInputFeeder.OnMove?.Invoke(blackboardLinker.moveDir);
                    if (blackboardLinker.IsDonePatrol())
                    {
                        SwitchToState(FSMState.CurrentState.Idle);
                    }
                    break;

                    // case FSMState.CurrentState.Combat:
                    //     // If player currently not visible, start lost sight timer; otherwise reset
                    //     if (!blackboardLinker?.isPlayerVisible ?? true)
                    //     {
                    //         if (_lostSightStart < 0f) _lostSightStart = Time.time;
                    //         else if (Time.time - _lostSightStart >= lostSightTimeout)
                    //         {
                    //             // Timed out -> go back to patrol
                    //             SwitchToState(FSMState.CurrentState.Patrol);
                    //         }
                    //     }
                    //     else
                    //     {
                    //         _lostSightStart = -1f;
                    //     }
                    //     break;
            }
        }

        bool IsIdleTimeout()
        {
            return Time.time - _stateEnterTime >= idleDuration;
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

        /// <summary>
        /// Switches the FSM to a new state and activates the corresponding Behavior Designer Behavior.
        /// </summary>
        /// <param name="newState">Target state</param>
        public void SwitchToState(FSMState.CurrentState newState)
        {
            if (_state == newState) return;

            // stop any previously active Behavior
            if (_activeBehavior != null)
            {
                StopBehavior(_activeBehavior);
                _activeBehavior = null;
            }

            _state = newState;
            _stateEnterTime = Time.time;
            // _lostSightStart = -1f;

            // choose and start the matching Behavior
            switch (newState)
            {
                case FSMState.CurrentState.Idle:
                    Debug.Log("Entering Idle State");
                    StartBehavior(idleBehavior);
                    break;
                case FSMState.CurrentState.Patrol:
                    Debug.Log("Entering Patrol State");
                    StartBehavior(patrolBehavior);
                    break;
                    // case FSMState.CurrentState.Combat:
                    //     Debug.Log("Entering Combat State");
                    //     StartBehavior(combatBehavior);
                    //     break;
            }

            // Debug log for developer convenience
            // Debug.Log($"[BotController] Switched to {_state}");
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
