using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine.AI;
using AIBot;

namespace CustomTask
{
	[TaskCategory("Custom")]
	public class CustomSeek : Action
	{
		[Tooltip("The GameObject that the agent is seeking")]
		[SerializeField] SharedGameObject target;
		[Tooltip("If target is null then use the target data")]
		[SerializeField] SharedTPointData currentTacticalPoint;
		[Tooltip("If target data is null then use the target position")]
		[SerializeField] SharedVector3 targetPosition;

		[Tooltip("Góc tối thiểu để cho phép di chuyển (độ). Nếu góc lớn hơn, bot chỉ xoay không di chuyển")]
		[SerializeField] float minAngleToMove = 60f;

		[SerializeField] SharedVector3 moveDir;
		[SerializeField] SharedVector3 lookEuler;
		[SerializeField] float repathInterval = 0.5f;

		PlayerRoot playerRoot;
		Transform playerCameraTarget;

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

			playerRoot = transform.root.GetComponent<PlayerRoot>();
			playerCameraTarget = playerRoot.PlayerCamera.GetPlayerCameraTarget();
		}

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (HasArrived())
			{
				moveDir.Value = Vector3.zero;
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

			// Tính toán hướng nhìn (luôn luôn tính, bất kể góc như thế nào)
			if (dir.magnitude > 0.01f)
			{
				Quaternion lookRotation = Quaternion.LookRotation(dir.normalized);
				lookEuler.Value = new Vector3(
					lookRotation.eulerAngles.x,
					lookRotation.eulerAngles.y,
					lookEuler.Value.z
				);
			}

			// Kiểm tra góc giữa hướng bot và hướng cần đi
			float angleToTarget = Vector3.Angle(playerCameraTarget.forward, dir.normalized);

			// Nếu góc quá lớn (ví dụ đi ngược), chỉ xoay không di chuyển
			if (angleToTarget > minAngleToMove)
			{
				// Góc quá lớn -> set moveDir = zero để bot chỉ xoay
				moveDir.Value = Vector3.zero;
			}
			else
			{
				// Góc OK -> cho phép di chuyển
				moveDir.Value = dir;
			}

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
			if (target.Value == null &&
			!currentTacticalPoint.Value.IsValid() &&
			targetPosition.Value == Vector3.zero) return;

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
			Vector3 targetPos = Vector3.zero;
			if (target.Value != null)
			{
				targetPos = target.Value.transform.position;
			}
			else if (currentTacticalPoint.Value.IsValid())
			{
				targetPos = currentTacticalPoint.Value.Position;
			}
			else if (targetPosition.Value != Vector3.zero)
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
			else if (currentTacticalPoint.Value.IsValid())
			{
				targetPos = currentTacticalPoint.Value.Position;
				hasTarget = true;
			}
			else if (targetPosition.Value != Vector3.zero)
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

			if (currentTacticalPoint.Value.IsValid())
			{
				DrawDirectionArrow(currentTacticalPoint.Value.Position, currentTacticalPoint.Value.Rotation);
			}
		}

		void DrawDirectionArrow(Vector3 position, Quaternion rotation)
		{
			// Lấy hướng forward từ rotation
			Vector3 forward = rotation * Vector3.forward;

			// Vẽ mũi tên chính (hướng di chuyển)
			Vector3 arrowStart = position + Vector3.up * 0.2f; // Nâng lên một chút để dễ nhìn
			Vector3 arrowEnd = arrowStart + forward * 1.5f; // Chiều dài mũi tên

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(arrowStart, arrowEnd);

			// Vẽ đầu mũi tên (2 nhánh tạo góc 30°)
			float arrowHeadLength = 0.4f;
			float arrowHeadAngle = 25f;

			Vector3 right = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
			Vector3 left = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

			Gizmos.DrawLine(arrowEnd, arrowEnd + right * arrowHeadLength);
			Gizmos.DrawLine(arrowEnd, arrowEnd + left * arrowHeadLength);
		}

		public override void OnEnd()
		{
			base.OnEnd();

			moveDir.Value = Vector3.zero;
			path.ClearCorners();
		}
	}
}