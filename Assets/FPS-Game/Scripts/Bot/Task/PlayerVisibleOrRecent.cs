using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace AIBot
{
    /// <summary>
    /// PlayerVisibleOrRecent: Conditional that returns Success when either:
    ///  - isPlayerVisible == true
    ///  - OR Time.time - playerLastSeenTime <= visibilityTimeout
    /// This smooths combat retention after brief occlusions.
    /// </summary>
    [TaskCategory("Combat/Condition")]
    [TaskDescription("Success while player is visible OR within visibilityTimeout seconds since last seen.")]
    public class PlayerVisibleOrRecent : Conditional
    {
        [Tooltip("Perception flag")]
        public SharedBool isPlayerVisible;

        [Tooltip("Last seen time (Time.time)")]
        public SharedFloat playerLastSeenTime;

        [Tooltip("Visibility timeout (seconds)")]
        public SharedFloat visibilityTimeout = 2f;

        public override TaskStatus OnUpdate()
        {
            // Visible right now -> success
            if (isPlayerVisible != null && isPlayerVisible.Value) return TaskStatus.Success;

            // If we have a last-seen timestamp, check timeout
            if (playerLastSeenTime != null && visibilityTimeout != null)
            {
                float last = playerLastSeenTime.Value;
                if (Time.time - last <= visibilityTimeout.Value) return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            isPlayerVisible = false;
            playerLastSeenTime = 0f;
            visibilityTimeout = 2f;
        }
    }
}
