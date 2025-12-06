using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine.AI;

namespace CustomTask
{
	[TaskCategory("Custom/Movement")]
	public class CustomSeek : Action
	{
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;
		public SharedVector3 moveDir;
		public float repathInterval = 0.5f;

		NavMeshAgent navMeshAgent;
		NavMeshPath path;
		int currentCorner = 0;
		float repathTimer = 0f;
		bool hasArrived = false;

		bool HasArrived() { return hasArrived; }

		public override void OnAwake()
		{
			base.OnAwake();
			navMeshAgent = transform.root.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			path = new NavMeshPath();
		}

		public override void OnStart()
		{
			base.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (HasArrived())
			{
				moveDir.Value = Vector3.zero;
				navMeshAgent.enabled = false;
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
			// dir.y = 0;

			moveDir.Value = dir;

			if (dir.magnitude < 0.2f)
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
			if (target == null) return;

			// Tính lại path
			if (NavMesh.CalculatePath(transform.position, target.Value.transform.position, NavMesh.AllAreas, path))
				currentCorner = 1; // corner[0] là vị trí hiện tại
			else
			{
				Debug.Log("Không thể tính toán path mới");
			}
		}

		public override void OnReset()
		{
			base.OnReset();
			target = null;
		}
	}
}