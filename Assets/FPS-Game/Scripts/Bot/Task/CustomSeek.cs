using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime.Tasks.Movement;

namespace CustomTask
{
	public class CustomSeek : NavMeshMovement
	{
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;
		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;

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