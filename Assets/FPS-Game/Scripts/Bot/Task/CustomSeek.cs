using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine.AI;
using AIBot;

namespace CustomTask
{
    public class CustomSeek : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedGameObject target;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private BotController botController;  // Declared here (private)

        public override void OnAwake()
        {
            // Auto-find BotController in hierarchy (child prefab)
            botController = Owner.gameObject.GetComponentInParent<BotController>();
            if (botController != null)
            {
                navMeshAgent = botController.navMeshAgent;
            }
            else
            {
                // Fallback: Direct fetch from root (PlayerPrefab)
                navMeshAgent = transform.root.GetComponent<NavMeshAgent>();
            }

            if (navMeshAgent == null)
            {
                Debug.LogError("CustomSeek: No NavMeshAgent found in hierarchy!");
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            if (navMeshAgent == null)
			{
				// return TaskStatus.Failure;
				Debug.Log("navMeshAgent = null");
            }
            SetDestination(Target());
        }

        public override TaskStatus OnUpdate()
        {
            if (navMeshAgent == null) return TaskStatus.Failure;  // Early exit

            if (HasArrived())
            {
                return TaskStatus.Success;
            }

            SetDestination(Target());
            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target.Value != null)
            {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}