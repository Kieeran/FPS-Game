using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
    public List<Transform> waypoints;
    public int index;
    public CharacterController controller;
    public NavMeshAgent navMeshAgent;
    public bool useInputFeeder;
    public float moveSpeed;
    public float repathInterval = 0.5f; // Cập nhật path mỗi 0.5 giây

    NavMeshPath path;
    int currentCorner = 0;
    float repathTimer = 0f;

    float rotationVelocity;
    Vector2 move;

    void Start()
    {
        if (!useInputFeeder)
        {
            navMeshAgent.SetDestination(waypoints[index].position);
        }

        else
        {
            path = new NavMeshPath();
        }
    }

    void Update()
    {
        if (!useInputFeeder) return;
        Move();
    }

    void Move()
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

        if (dir.magnitude < 0.2f)
        {
            move = Vector2.zero;
            // Đến corner -> sang corner tiếp theo
            if (currentCorner < path.corners.Length - 1)
                currentCorner++;
            else
                return; // Đến đích
        }
        else
        {
            Vector3 moveDir = dir.normalized;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );
            }

            Vector3 velocity = moveDir * moveSpeed;
            velocity.y += -9.81f;

            controller.Move(velocity * Time.deltaTime);
        }
    }

    void CalculateNewPath()
    {
        if (waypoints[index] == null) return;

        // Tính lại path
        if (NavMesh.CalculatePath(transform.position, waypoints[index].position, NavMesh.AllAreas, path))
            currentCorner = 1; // corner[0] là vị trí hiện tại
        else
        {
            Debug.Log("Không xảy ra");
        }
    }

    void OnDrawGizmos()
    {
        if (path != null && path.corners.Length > 1)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.corners.Length - 1; i++)
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
        }
    }
}