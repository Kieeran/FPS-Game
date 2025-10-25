using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace AIBot
{
    /// <summary>
    /// Behavior Designer Action task.
    /// Copies the target Transform's position into a SharedVector3 (playerLastSeenPos)
    /// and writes the current Time.time into a SharedFloat (playerLastSeenTime).
    ///
    /// Designed to be cheap and safe: returns Success when update occurred, Failure when
    /// the target is not available or optional gating conditions are not met.
    /// Typical placement: inside CombatTree, in parallel with a continuous Chase task.
    /// </summary>
    [TaskCategory("Combat/Utility")]
    [TaskDescription("Update the last-seen player position (playerLastSeenPos) and timestamp (playerLastSeenTime) from the target transform.")]
    public class UpdateLastSeen : Action
    {
        [Tooltip("The observed target transform (usually targetPlayer).")]
        public SharedGameObject target;

        [Tooltip("SharedVector3 to write the target position into (playerLastSeenPos).")]
        public SharedVector3 lastSeenPos;

        [Tooltip("SharedFloat to write the Time.time into (playerLastSeenTime).")]
        public SharedFloat lastSeenTime;

        [Tooltip("Optional: if true, only update when isPlayerVisible is true (if provided).")]
        public bool onlyWhenVisible = false;

        [Tooltip("Optional SharedBool that indicates current perception (isPlayerVisible). Used when onlyWhenVisible=true.")]
        public SharedBool isPlayerVisible;

        /// <summary>
        /// Called when the task starts. Nothing to cache — this task reads directly from SharedVariables.
        /// </summary>
        public override void OnStart()
        {
            // Intentionally empty — no setup required.
        }

        /// <summary>
        /// Performs the update operation.
        /// Returns Success if values were written; Failure when target is missing or gating prevents update.
        /// </summary>
        /// <returns>TaskStatus.Success or TaskStatus.Failure</returns>
        public override TaskStatus OnUpdate()
        {
            // Optional gating by visibility
            if (onlyWhenVisible)
            {
                if (isPlayerVisible == null || !isPlayerVisible.Value)
                {
                    // Not allowed to update when not visible
                    return TaskStatus.Failure;
                }
            }

            if (target == null || target.Value == null)
            {
                // No target to read from
                return TaskStatus.Failure;
            }

            // Write position and timestamp into SharedVariables (if assigned)
            if (lastSeenPos != null)
            {
                lastSeenPos.Value = target.Value.transform.position;
            }

            if (lastSeenTime != null)
            {
                lastSeenTime.Value = Time.time;
            }

            return TaskStatus.Success;
        }

        /// <summary>
        /// Called when the task ends (child stops or the parent ends).
        /// Nothing to clean up.
        /// </summary>
        public override void OnEnd()
        {
            // Nothing to clean up for this lightweight updater.
        }

        /// <summary>
        /// Reset inspector-exposed fields to sane defaults for reusing the node in editor.
        /// </summary>
        public override void OnReset()
        {
            target = null;
            lastSeenPos = null;
            lastSeenTime = null;
            onlyWhenVisible = false;
            isPlayerVisible = null;
        }
    }
}
