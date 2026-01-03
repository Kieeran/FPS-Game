using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class SearchSector
{
    public int sectorIndex;
    public float fromAngle;
    public float toAngle;

    public List<Vector3> sampledPoints = new();
    public List<Vector3> validHidingPoints = new();

    public float score;
}

public class SmartChasing : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Sector Settings")]
    public int sectorCount;
    public float viewAngle;

    public float searchRadius;
    public float radialSamples;

    [Header("NavMesh Sampling")]
    public int samplesPerSector;
    public float navMeshSampleDistance;

    [Header("Path Validation")]
    public float maxPathDistance;
    public bool requireCompletePath = true;

    [Header("Visibility")]
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool drawGizmos = true;

    private List<SearchSector> sectors = new();

    [ContextMenu("Generate Sectors")]
    void GenerateSectors()
    {
        sectors.Clear();

        // 1. Lấy hướng cuối cùng của player
        Vector3 referenceDir = target.forward;
        referenceDir.y = 0;
        float baseAngle = Mathf.Atan2(referenceDir.x, referenceDir.z) * Mathf.Rad2Deg;

        // 2. Chia sector quanh hướng này
        float angleStep = viewAngle / sectorCount;
        float startAngle = baseAngle - viewAngle * 0.5f;

        for (int i = 0; i < sectorCount; i++)
        {
            float from = startAngle + i * angleStep;
            float to = from + angleStep;

            sectors.Add(new SearchSector
            {
                sectorIndex = i,
                fromAngle = from,
                toAngle = to
            });
        }

        Debug.Log("[SectorSearch] Generated sectors aligned to LKP direction");
    }

    [ContextMenu("Sample NavMesh Per Sector")]
    void SampleNavMeshPerSector()
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        foreach (var sector in sectors)
        {
            sector.sampledPoints.Clear();
            sector.validHidingPoints.Clear();

            for (int r = 1; r <= radialSamples; r++)
            {
                float radiusT = (float)r / radialSamples;
                float currentRadius = searchRadius * radiusT;

                for (int i = 0; i < samplesPerSector; i++)
                {
                    float t = (i + 0.5f) / samplesPerSector;
                    float angle = Mathf.Lerp(sector.fromAngle, sector.toAngle, t);

                    Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                    Vector3 theoreticalPoint = target.position + dir * currentRadius;

                    if (NavMesh.SamplePosition(
                        theoreticalPoint,
                        out NavMeshHit hit,
                        navMeshSampleDistance,
                        NavMesh.AllAreas))
                    {
                        sector.sampledPoints.Add(hit.position);
                    }
                }
            }
        }

        Debug.Log("[SectorSearch] NavMesh sampling completed");
    }

    // [ContextMenu("Evaluate Sectors")]
    // void EvaluateSectors()
    // {
    //     foreach (var sector in sectors)
    //     {
    //         sector.validHidingPoints.Clear();
    //         sector.score = 0f;

    //         foreach (var p in sector.sampledPoints)
    //         {
    //             if (!IsValidHidingPoint(p))
    //                 continue;

    //             sector.validHidingPoints.Add(p);
    //         }

    //         // Score đơn giản (có thể mở rộng cho thesis)
    //         sector.score = sector.validHidingPoints.Count;
    //     }

    //     Debug.Log("[SectorSearch] Sector evaluation completed");
    // }

    // bool IsValidHidingPoint(Vector3 point)
    // {
    //     // 1. Bot KHÔNG nhìn thấy
    //     if (HasLineOfSight(transform.position, point))
    //         return false;

    //     // 2. Target có thể đi tới
    //     if (!HasValidPath(target.position, point))
    //         return false;

    //     return true;
    // }

    // bool HasLineOfSight(Vector3 from, Vector3 to)
    // {
    //     Vector3 dir = (to - from).normalized;
    //     float dist = Vector3.Distance(from, to);
    //     return !Physics.Raycast(from, dir, dist, obstacleMask);
    // }

    // bool HasValidPath(Vector3 from, Vector3 to)
    // {
    //     NavMeshPath path = new NavMeshPath();
    //     if (!NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path))
    //         return false;

    //     if (requireCompletePath && path.status != NavMeshPathStatus.PathComplete)
    //         return false;

    //     if (path.status == NavMeshPathStatus.PathInvalid)
    //         return false;

    //     float length = 0f;
    //     for (int i = 1; i < path.corners.Length; i++)
    //     {
    //         length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
    //     }

    //     return length <= maxPathDistance;
    // }

    void OnDrawGizmos()
    {
        if (!drawGizmos || sectors == null) return;

        Vector3 origin = target != null ? target.position : Vector3.zero;

        foreach (var sector in sectors)
        {
            DrawSector(origin, sector);

            Gizmos.color = Color.green;
            foreach (var p in sector.sampledPoints)
                Gizmos.DrawSphere(p, 0.15f);

            Gizmos.color = Color.cyan;
            foreach (var p in sector.validHidingPoints)
                Gizmos.DrawSphere(p, 0.2f);
        }

        DrawLineToTarget();
    }

    void DrawSector(Vector3 origin, SearchSector sector)
    {
        int steps = 16;
        float angleStep = (sector.toAngle - sector.fromAngle) / steps;

        Gizmos.color = Color.blue;

        // =========================
        // 1. Vẽ các vòng bán kính
        // =========================
        for (int r = 1; r <= radialSamples; r++)
        {
            float radiusT = (float)r / radialSamples;
            float currentRadius = searchRadius * radiusT;

            Vector3 prev = origin + Quaternion.Euler(0, sector.fromAngle, 0)
                * Vector3.forward * currentRadius;

            for (int i = 1; i <= steps; i++)
            {
                float angle = sector.fromAngle + angleStep * i;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 point = origin + dir * currentRadius;

                Gizmos.DrawLine(prev, point);
                prev = point;
            }
        }

        // =========================
        // 2. Vẽ biên sector (2 cạnh)
        // =========================
        Gizmos.DrawLine(origin,
            origin + Quaternion.Euler(0, sector.fromAngle, 0)
            * Vector3.forward * searchRadius);

        Gizmos.DrawLine(origin,
            origin + Quaternion.Euler(0, sector.toAngle, 0)
            * Vector3.forward * searchRadius);
    }

    void DrawLineToTarget()
    {
        if (target == null) return;

        // Mặc định là XANH (không có vật cản)
        Color lineColor = Color.green;

        Vector3 dir = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // NẾU có vật cản → đổi sang VÀNG
        if (Physics.Raycast(transform.position, dir, distanceToTarget, obstacleMask))
        {
            lineColor = Color.yellow;
        }

        Gizmos.color = lineColor;
        Gizmos.DrawLine(transform.position, target.position);

        // Vẽ sphere tại target
        Gizmos.color = lineColor * 0.7f;
        Gizmos.DrawWireSphere(target.position, 0.3f);
    }
}