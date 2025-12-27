using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine.AI;

namespace CustomTask
{
	[TaskCategory("Custom")]
	public class CustomSeek : Action
	{
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;
		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;

		public SharedVector3 moveDir;
		public float repathInterval = 0.5f;

		NavMeshPath path;
		int currentCorner = 0;
		float repathTimer = 0f;
		bool hasArrived = false;

		bool HasArrived() { return hasArrived; }

		public override void OnStart()
		{
			base.OnStart();
			if (path == null) path = new NavMeshPath();

			hasArrived = false;
			currentCorner = 0;
			repathTimer = 0f;
		}

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (HasArrived())
			{
				moveDir.Value = Vector3.zero;
				// navMeshAgent.enabled = false;
				return TaskStatus.Success;
			}
			CalculatePath();
			return TaskStatus.Running;
		}

		void CalculatePath()
		{
			repathTimer += Time.deltaTime;
			if (repathTimer >= repathInterval)
			{
				CalculateNewPath();
				repathTimer = 0f;
			}

			if (path == null || path.corners.Length == 0) return;

			// Di chuyển theo corner hiện tại
			Vector3 nextPoint = path.corners[currentCorner];
			Vector3 dir = nextPoint - transform.position;
			dir.y = 0;

			moveDir.Value = dir;

			if (dir.magnitude < 0.25f)
			{
				// Đến corner -> sang corner tiếp theo
				if (currentCorner < path.corners.Length - 1)
				{
					currentCorner++;
				}
				else
				{
					hasArrived = true;
					return; // Đến đích
				}
			}
		}

		void CalculateNewPath()
		{
			if (target.Value == null && targetPosition.Value == Vector3.zero) return;

			// Tính lại path
			if (NavMesh.CalculatePath(transform.position, Target(), NavMesh.AllAreas, path))
				currentCorner = 1; // corner[0] là vị trí hiện tại
			else
			{
				Debug.Log("Không thể tính toán path mới");
			}
		}

		// Return targetPosition if target is null
		private Vector3 Target()
		{
			Vector3 targetPos;
			if (target.Value != null)
			{
				targetPos = target.Value.transform.position;
			}
			else
			{
				targetPos = targetPosition.Value;
			}

			float snapDistance = 10f; // Khoảng cách tìm kiếm xuống dưới
			if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, snapDistance, NavMesh.AllAreas))
			{
				targetPos = hit.position; // Trả về điểm trên NavMesh
			}

			// Nếu không tìm thấy, trả về vị trí gốc
			return targetPos;
		}

		public override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (path != null && path.corners.Length > 1)
			{
				Gizmos.color = Color.cyan;
				for (int i = 0; i < path.corners.Length - 1; i++)
					Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
			}

			// Vẽ target mà bot đi tới
			DrawTarget();
		}

		void DrawTarget()
		{
			Vector3 targetPos = Vector3.zero;
			bool hasTarget = false;

			if (target != null && target.Value != null)
			{
				targetPos = target.Value.transform.position;
				hasTarget = true;
			}
			else if (targetPosition != null && targetPosition.Value != Vector3.zero)
			{
				targetPos = targetPosition.Value;
				hasTarget = true;
			}

			if (!hasTarget) return;

			// Vẽ sphere tại vị trí target
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(targetPos, 0.5f);

			// Vẽ arrow chỉ xuống target
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(targetPos + Vector3.up * 2f, targetPos);

			// Vẽ X marking the spot
			Gizmos.color = Color.green;
			Gizmos.DrawLine(targetPos + new Vector3(-0.3f, 0.1f, -0.3f), targetPos + new Vector3(0.3f, 0.1f, 0.3f));
			Gizmos.DrawLine(targetPos + new Vector3(-0.3f, 0.1f, 0.3f), targetPos + new Vector3(0.3f, 0.1f, -0.3f));
		}

		public override void OnEnd()
		{
			base.OnEnd();
			moveDir.Value = Vector3.zero;
		}
	}
}