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

    public bool showNavigationGraph = true;

    private void OnDrawGizmos()
    {
        if (!showNavigationGraph) return;
        NavMeshPath path = new();
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