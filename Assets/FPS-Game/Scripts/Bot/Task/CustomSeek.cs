using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine.AI;

namespace CustomTask
{
	[TaskCategory("Custom/Movement")]
	[TaskDescription("Outputs a movement direction (moveDir) for the AI based on NavMesh pathfinding to the target.")]
	public class CustomSeek : NavMeshMovement
	{
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;
		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;
		public SharedVector2 moveDir;

		public override void OnAwake()
		{
			navMeshAgent = transform.root.GetComponent<NavMeshAgent>();
			navMeshAgent.updatePosition = false;
			navMeshAgent.updateRotation = false;
		}

		public override void OnStart()
		{
			base.OnStart();

			SetDestination(Target());
		}

		// Seek the destination. Return success once the agent has reached the destination.
		// Return running if the agent hasn't reached the destination yet
		public override TaskStatus OnUpdate()
		{
			if (HasArrived())
			{
				moveDir.Value = Vector2.zero;
				return TaskStatus.Success;
			}

			// chỉ update destination khi cần
			if (!navMeshAgent.hasPath ||
				navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
			{
				SetDestination(Target());
			}

			// dùng steeringTarget thay cho nextPosition
			Vector3 target = navMeshAgent.steeringTarget;
			Vector3 worldDir = target - navMeshAgent.transform.position;
			worldDir.y = 0;

			if (worldDir.sqrMagnitude < 0.05f)
				moveDir.Value = Vector2.zero;
			else
			{
				worldDir.Normalize();

				float forward = Vector3.Dot(navMeshAgent.transform.forward, worldDir);
				float right = Vector3.Dot(navMeshAgent.transform.right, worldDir);

				moveDir.Value = new Vector2(right, forward);
			}

			// đồng bộ agent với character controller
			navMeshAgent.nextPosition = navMeshAgent.transform.position;

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