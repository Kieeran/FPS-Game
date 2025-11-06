using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using AIBot;

namespace CustomTask
{
	[TaskCategory("Custom/Movement")]
	[TaskDescription("Outputs a movement direction (moveDir) for the AI based on NavMesh pathfinding to the target.")]
	public class CustomSeek : Action
	{
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;

		[Tooltip("If target is null then use the target position instead")]
		public SharedVector3 targetPosition;

		[Tooltip("Output: normalized movement direction (x,z)")]
		public SharedVector2 MoveDirection;

		private Transform ownerTransform;

		public override void OnAwake()
		{
			ownerTransform = Owner.transform.root;

			SharedVariable globalVar = GlobalVariables.Instance.GetVariable("MoveDirection");
			if (globalVar is SharedVector2 globalMoveDir)
			{
				MoveDirection = globalMoveDir;
			}
			else
			{
				Debug.LogError("CustomSeek: Không tìm thấy biến Global 'MoveDirection'!");
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (ownerTransform == null)
				return TaskStatus.Failure;

			Vector2 dir = Vector2.zero;

			if (target.Value != null)
			{
				dir = InGameManager.Instance.PathFinding(ownerTransform, target.Value.transform);
			}
			else
			{
				Vector3 tempPos = targetPosition.Value;
				GameObject temp = new GameObject("TempTarget");
				temp.transform.position = tempPos;
				dir = InGameManager.Instance.PathFinding(ownerTransform, temp.transform);
				Object.Destroy(temp);
			}

			MoveDirection.Value = dir;

			if (target.Value != null &&
				Vector3.Distance(ownerTransform.position, target.Value.transform.position) < 1.5f)
			{
				MoveDirection.Value = Vector2.zero;
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		public override void OnReset()
		{
			target = null;
			targetPosition = Vector3.zero;
			MoveDirection = Vector2.zero;
		}
	}
}