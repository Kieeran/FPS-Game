using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }
    public List<Zone> allZones = new();
    public float heightOffset = 2.84f;

    public LayerMask obstacleLayer;
    [SerializeField] LayerMask zoneLayer;
    [SerializeField] Transform zoneContainer;

    public Dictionary<ZoneID, Zone> zoneCache;

    // Chạy cả Edit Mode & Play Mode
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
#if UNITY_EDITOR
        else if (Instance != this)
        {
            Debug.LogWarning(
                "Multiple ZoneManager detected in scene. Only one should exist.",
                this);
        }
#endif

        BuildZoneCache();
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    // Runtime-only logic
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnValidate()
    {
        if (zoneContainer != null)
        {
            allZones = zoneContainer.GetComponentsInChildren<Zone>(true).ToList();
        }
    }

    public Zone GetZoneByID(ZoneID zoneID)
    {
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData.zoneID == zoneID) return zone;
        }

        return null;
    }

    public Zone GetZoneAt(Transform pointTF, Vector3 pointPos)
    {
        if (pointTF == null)
        {
            if (allZones == null || allZones.Count == 0) return null;

            foreach (Zone zone in allZones)
            {
                foreach (var col in zone.colliders)
                {
                    // Kiểm tra nếu vị trí điểm xét nằm trong vùng của Collider
                    if (col.bounds.Contains(pointPos))
                    {
                        return zone;
                    }
                }
            }

            return null;
        }

        else
        {
            // Lấy bán kính từ SphereCollider của Object hoặc lấy scale
            float radius = 1.0f;
            if (pointTF.TryGetComponent<SphereCollider>(out var sphereCol))
            {
                radius = sphereCol.radius * pointTF.transform.lossyScale.x;
            }

            Collider[] hitColliders = Physics.OverlapSphere(pointTF.transform.position, radius, zoneLayer);
            if (hitColliders.Length == 0) return null;

            Zone zone = hitColliders[0].GetComponentInParent<Zone>();
            return zone;
        }
    }

    public void BuildZoneCache()
    {
        zoneCache = new();
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData != null && zone.zoneData.zoneID != ZoneID.None)
            {
                if (!zoneCache.ContainsKey(zone.zoneData.zoneID))
                    zoneCache.Add(zone.zoneData.zoneID, zone);
            }
        }
    }

    public Vector3 GetSnappedPos(Vector3 originalPos)
    {
        if (NavMesh.SamplePosition(originalPos, out NavMeshHit hit, 4.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        Debug.Log("Không snap được");
        return originalPos;
    }

    [ContextMenu("Bake All Portal Traversal Cost")]
    public void BakeAllPortalTraversalCost()
    {
        int successCount = 0;
        NavMeshPath path = new();

        foreach (Zone zone in allZones)
        {
            if (zone.zoneData == null) continue;

            foreach (PortalPoint portal in zone.zoneData.portals)
            {
                ZoneData zoneA = portal.zoneDataA;
                ZoneData zoneB = portal.zoneDataB;
                if (zoneA == null || zoneB == null) continue;

                float dist1 = GetNavMeshDistance(GetSnappedPos(zoneA.centerPos), GetSnappedPos(portal.position), path);
                float dist2 = GetNavMeshDistance(GetSnappedPos(portal.position), GetSnappedPos(zoneB.centerPos), path);

                // Lưu tổng quãng đường thực tế
                portal.traversalCost = dist1 + dist2;
                successCount++;
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(zone.zoneData);
#endif
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        Debug.Log($"Bake hoàn tất! Đã cập nhật NavMesh Distance cho {successCount} portal.");
    }

    // Tính chiều dài đường đi NavMesh
    private float GetNavMeshDistance(Vector3 start, Vector3 end, NavMeshPath path)
    {
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            float distance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return distance;
        }

        Debug.LogWarning($"Không tìm thấy đường NavMesh giữa {start} và {end}.");
        return 0;
    }

    public ZoneData startZone;
    public ZoneData targetZone;
    public bool showResultRoute = true;
    List<ZoneData> route = new();

    [ContextMenu("Get Shortest Path")]
    public void CalculateShortestPath()
    {
        route = GetShortestPath(startZone, targetZone);

        if (route == null || route.Count == 0)
            Debug.LogWarning("Không tìm thấy lộ trình!");
        else
            Debug.Log($"Đã tìm thấy lộ trình qua {route.Count} Zone.");
    }

    public List<ZoneData> GetShortestPath(ZoneData startZone, ZoneData targetZone)
    {
        if (startZone == null || targetZone == null) return new List<ZoneData>();

        Dictionary<ZoneData, float> dists = new();
        Dictionary<ZoneData, ZoneData> prevs = new();
        List<(ZoneData zone, float d)> pq = new();

        // Khởi tạo
        foreach (var z in allZones)
        {
            dists[z.zoneData] = float.MaxValue;
            prevs[z.zoneData] = null;
        }

        dists[startZone] = 0;
        pq.Add((startZone, 0));

        while (pq.Count > 0)
        {
            pq.Sort((a, b) => a.d.CompareTo(b.d));

            (ZoneData u, float d) = pq[0];
            pq.RemoveAt(0);

            if (d > dists[u]) continue;
            if (u == targetZone) break;

            foreach (var portal in u.portals)
            {
                ZoneData v = portal.GetOtherZone(u);
                float weight = portal.traversalCost; // traversalCost đã bake

                if (dists[u] + weight < dists[v])
                {
                    dists[v] = dists[u] + weight;
                    prevs[v] = u;
                    pq.Add((v, dists[v]));
                }
            }
        }

        return ReconstructPath(prevs, targetZone);
    }

    private List<ZoneData> ReconstructPath(Dictionary<ZoneData, ZoneData> prevs, ZoneData target)
    {
        List<ZoneData> path = new();
        ZoneData curr = target;

        while (curr != null)
        {
            path.Add(curr);
            // Kiểm tra xem node hiện tại có nằm trong bảng truy vết không
            if (prevs.ContainsKey(curr))
                curr = prevs[curr];
            else
                break;
        }

        path.Reverse();
        return path;
    }

    // Hàm phụ trợ tìm Portal nối giữa 2 Zone
    private PortalPoint GetPortalBetween(ZoneData a, ZoneData b)
    {
        foreach (var p in a.portals)
        {
            if (p.GetOtherZone(a) == b) return p;
        }

        Debug.Log($"Không tìm thấy portal nối giữa {a.zoneID} và {b.zoneID}");
        return null;
    }

    public bool showNavigationGraph = true;

    private void OnDrawGizmos()
    {
        NavMeshPath path = new();

        if (showNavigationGraph)
        {
            foreach (Zone zone in allZones)
            {
                if (zone.zoneData == null) continue;
                Vector3 startPos = zone.zoneData.centerPos;
                if (zone.zoneData.portals == null) continue;

                foreach (var portal in zone.zoneData.portals)
                {
                    if (portal == null) continue;

                    // Vẽ đường đi dưới đất từ Center -> Portal
                    DrawNavMeshGizmoLine(GetSnappedPos(startPos), GetSnappedPos(portal.position), path, Color.green);
                }
            }
        }

        if (showResultRoute)
        {
            if (route != null && route.Count >= 2)
            {
                for (int i = 0; i < route.Count - 1; i++)
                {
                    ZoneData current = route[i];
                    ZoneData next = route[i + 1];
                    PortalPoint connector = GetPortalBetween(current, next);

                    if (connector != null)
                    {
                        DrawNavMeshGizmoLine(GetSnappedPos(current.centerPos), GetSnappedPos(connector.position), path, Color.green);
                        DrawNavMeshGizmoLine(GetSnappedPos(connector.position), GetSnappedPos(next.centerPos), path, Color.green);
                    }
                }
            }
        }
    }

    // Hàm phụ trợ vẽ đường gấp khúc dựa trên NavMesh Corners
    private void DrawNavMeshGizmoLine(Vector3 start, Vector3 end, NavMeshPath path, Color color)
    {
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            Gizmos.color = color;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);

                // Vẽ các điểm nút trên đường đi để dễ quan sát
                Gizmos.DrawSphere(path.corners[i], 0.1f);
            }
        }
        else
        {
            // Nếu không có NavMesh, vẽ đường thẳng màu đỏ cảnh báo
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);
        }
    }
}