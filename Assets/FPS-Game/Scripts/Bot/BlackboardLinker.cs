using System;
using UnityEngine;
using BehaviorDesigner.Runtime;

namespace AIBot
{
    /// <summary>
    /// Minimal adapter that synchronizes a tiny set of blackboard fields to the active Behavior Designer Behavior.
    /// It performs change-detection to avoid unnecessary SetVariableValue calls.
    /// This expects Behavior trees to declare SharedVariables:
    ///   - SharedBool  "isPlayerVisible"
    ///   - SharedVector3 "playerLastSeenPos"
    ///   - SharedInt "currentWaypointIndex"
    ///   - SharedGameObjectList or SharedTransformList "waypoints" (optional authoring)
    /// </summary>
    [DisallowMultipleComponent]
    public class BlackboardLinker : MonoBehaviour
    {
        [Header("Runtime cache")]
        [Tooltip("Current active Behavior Designer Behavior component (set by BotController.BindToBehavior)")]
        [SerializeField] Behavior activeBehavior;

        // internal cached values to avoid setting BD vars every frame
        // private bool _isPlayerVisible = false;
        // private Vector3 _playerLastSeenPos = Vector3.zero;
        [SerializeField] Vector3 moveDir;
        [SerializeField] float targetPitch;

        /// <summary>Expose last seen pos for other systems (e.g., BotController).</summary>
        // public Vector3 PlayerLastSeenPos => _playerLastSeenPos;

        /// <summary>Quick accessor for visibility.</summary>
        // public bool isPlayerVisible => _isPlayerVisible;

        /// <summary>
        /// Bind the linker to a Behavior Designer Behavior (called when FSM switches states).
        /// Immediately seeds the BD variables with current C# blackboard values.
        /// </summary>
        /// <param name="behavior">Behavior component to bind to (may be null to unbind).</param>
        public void BindToBehavior(Behavior behavior)
        {
            activeBehavior = behavior;
            if (activeBehavior == null) return;
            // seed values right away
            // SafeSet("isPlayerVisible", _isPlayerVisible);
            // SafeSet("playerLastSeenPos", _playerLastSeenPos);

            switch (activeBehavior.BehaviorName)
            {
                case "IdleTree":
                    SafeSet("targetPitch", 0);
                    return;

                case "PatrolTree":
                    SafeSet("currentWayPoint", InGameManager.Instance.Waypoints.GetRandomWaypoint().gameObject);
                    return;

                default:
                    return;
            }
        }

        public Vector3 GetMovDir() { return moveDir; }
        public float GetTargetPitch() { return targetPitch; }

        void Update()
        {
            GetValuesSharedVariables();
        }

        void GetValuesSharedVariables()
        {
            if (!activeBehavior) return;

            string behaviorName = activeBehavior.BehaviorName;
            switch (behaviorName)
            {
                case "IdleTree":
                    targetPitch = (float)activeBehavior.GetVariable("targetPitch").GetValue();
                    return;

                case "PatrolTree":
                    moveDir = (Vector3)activeBehavior.GetVariable("moveDir").GetValue();
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Set player visibility and last seen pos. Called by PerceptionSensor (via BotController).
        /// </summary>
        // public void SetPlayerVisible(bool visible, Vector3 lastSeenPos, GameObject playerGameObject)
        // {
        //     _isPlayerVisible = visible;
        //     _playerLastSeenPos = lastSeenPos;

        //     // update BD Variables on active behavior
        //     SafeSet("isPlayerVisible", _isPlayerVisible);
        //     SafeSet("playerLastSeenPos", _playerLastSeenPos);

        //     // also expose the player transform if tasks expect it (SharedTransform named "targetPlayer")
        //     if (playerGameObject != null)
        //     {
        //         Debug.Log("playerGameObject is not null");
        //         SafeSet("targetPlayer", playerGameObject);
        //     }
        //     else
        //     {
        //         Debug.Log("playerGameObject is null");
        //         // clear targetPlayer if not visible (optional)
        //         SafeSet("targetPlayer", null);
        //     }
        // }

        /// <summary>
        /// Helper: set a variable on the active Behavior safely (if active and variable exists).
        /// Uses BehaviorDesigner.Runtime.Behavior.SetVariableValue which accepts (name, value).
        /// </summary>
        // Replace existing SafeSet with this implementation
        private void SafeSet(string variableName, object value)
        {
            if (activeBehavior == null) return;

            // Get the shared variable instance (may be null if tree omitted it)
            var sharedVar = activeBehavior.GetVariable(variableName);
            if (sharedVar == null) return;

            // boolean
            if (value is bool b && sharedVar is SharedBool sb)
            {
                if (sb.Value == b) return;
                sb.Value = b;
                return;
            }

            // int
            if (value is int i && sharedVar is SharedInt si)
            {
                if (si.Value == i) return;
                si.Value = i;
                return;
            }

            // float
            if (value is float f && sharedVar is SharedFloat sf)
            {
                if (Mathf.Approximately(sf.Value, f)) return;
                sf.Value = f;
                return;
            }

            // Vector3
            if (value is Vector3 v3 && sharedVar is SharedVector3 sv3)
            {
                if (sv3.Value == v3) return;
                sv3.Value = v3;
                return;
            }

            // Vector2
            if (value is Vector2 v2 && sharedVar is SharedVector2 sv2)
            {
                if (sv2.Value == v2) return;
                sv2.Value = v2;
                return;
            }

            // Transform
            if ((value == null || value is Transform) && sharedVar is SharedTransform st)
            {
                Transform t = value as Transform;
                if (st.Value == t) return;
                st.Value = t;
                return;
            }

            // GameObject
            if ((value == null || value is GameObject) && sharedVar is SharedGameObject sgo)
            {
                GameObject go = value as GameObject;
                if (sgo.Value == go) return;
                sgo.Value = go;
                return;
            }

            // Fallback: try BD API 'SetVariableValue' (last resort)
            try
            {
                activeBehavior.SetVariableValue(variableName, value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"BlackboardLinker.SafeSet: failed to set '{variableName}' - {ex.Message}");
            }
        }
    }
}
