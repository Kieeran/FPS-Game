using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TacticalPoints : MonoBehaviour
{
    public List<Transform> TPoints { get; private set; }
    [Header("Gizmos Settings")]
    public bool drawGizmos = true;
    public float gizmoRadius = 0.5f;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    void Awake()
    {
        foreach (Transform tp in transform)
        {
            TPoints.Add(tp);
        }
    }

    private void OnDrawGizmos()
    {
        if (TPoints == null) return;
        if (!drawGizmos) return;

        foreach (Transform tp in transform)
        {
            if (tp == null) continue;

            // Kiểm tra xem điểm này có nằm trên NavMesh không
            // Kiểm tra trong phạm vi 10m từ vị trí điểm
            bool isOnNavMesh = NavMesh.SamplePosition(tp.position, out NavMeshHit hit, 10f, NavMesh.AllAreas);

            if (isOnNavMesh)
            {
                // Nếu nằm trên NavMesh, vẽ màu xanh và đường nối tới vị trí snap
                Gizmos.color = validColor;
                Gizmos.DrawSphere(hit.position, gizmoRadius);

                // Nếu điểm gốc bị lệch, vẽ đường nối tới vị trí NavMesh thực tế
                Gizmos.DrawLine(tp.position, hit.position);
            }
            else
            {
                // Nếu không nằm trên NavMesh, vẽ màu đỏ cảnh báo
                Gizmos.color = invalidColor;
                Gizmos.DrawCube(tp.position, new Vector3(gizmoRadius, gizmoRadius, gizmoRadius));
            }
        }
    }
}
