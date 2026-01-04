using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ZonePortalsContainer : MonoBehaviour
{
    public ZonesContainer zonesContainer;
    [SerializeField] List<ZonePortal> zonePortals;
    public List<ZonePortal> GetZonePortals() { return zonePortals; }

    // Dictionary lưu trữ: ZoneID hiện tại -> Danh sách các Portal dẫn đi các Zone khác
    public Dictionary<ZoneID, List<ZonePortal>> zoneAdjacencyMap = new();

    void Awake()
    {
        RebuildAdjacencyMap();
    }

    void RebuildAdjacencyMap()
    {
        zoneAdjacencyMap.Clear();

        foreach (var zone in zonesContainer.GetZones())
        {
            if (!zoneAdjacencyMap.ContainsKey(zone.zoneID))
            {
                zoneAdjacencyMap[zone.zoneID] = new List<ZonePortal>();
            }

            // Copy từ zone.portals vào dictionary
            zoneAdjacencyMap[zone.zoneID].AddRange(zone.portals);
        }

        Debug.Log($"Rebuilt adjacency map with {zoneAdjacencyMap.Count} zones");
    }

    void OnValidate()
    {
        zonePortals = GetComponentsInChildren<ZonePortal>().ToList();
    }

    [ContextMenu("Scan All Portals")]
    public void ScanAllPortals()
    {
        zoneAdjacencyMap.Clear();
        ZonePortal[] allPortals = zonePortals.ToArray();

        foreach (var portal in allPortals)
        {
            // Đăng ký cho Zone A
            RegisterConnection(portal.zoneA.zoneID, portal);
            // Đăng ký cho Zone B (để Portal có tính 2 chiều)
            RegisterConnection(portal.zoneB.zoneID, portal);
        }

        foreach (var zone in zonesContainer.GetZones())
        {
            zone.portals.Clear();
            zone.portals = zoneAdjacencyMap[zone.zoneID];
        }

        Debug.Log($"Đã cập nhật bản đồ giao thông với {allPortals.Length} cổng kết nối.");
    }

    [ContextMenu("Bake Portal Strategy")]
    public void BakePortalStrategy()
    {
        foreach (ZonePortal portal in zonePortals)
        {
            // Bake cho cả 2 phía của Portal
            portal.nearestIPointInA = CalculateBestEntry(portal, portal.zoneA);
            portal.nearestIPointInB = CalculateBestEntry(portal, portal.zoneB);

            // Đánh dấu để Unity lưu lại thay đổi sau khi Bake
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        Debug.Log("Bake Portal Strategy thành công!");
    }

    private void RegisterConnection(ZoneID zoneID, ZonePortal portal)
    {
        if (!zoneAdjacencyMap.ContainsKey(zoneID))
        {
            zoneAdjacencyMap[zoneID] = new List<ZonePortal>();
        }
        zoneAdjacencyMap[zoneID].Add(portal);
    }

    private PointVisibilityData CalculateBestEntry(ZonePortal portal, Zone targetZone)
    {
        PointVisibilityData result = new();
        if (targetZone.visibilityMatrix == null || targetZone.visibilityMatrix.Count == 0) return result;

        int maxPriority = targetZone.visibilityMatrix.Max(p => p.priority);
        var highPriorityPoints = targetZone.visibilityMatrix
            .Where(p => p.priority >= maxPriority)
            .ToList();

        PointVisibilityData bestPoint = new();
        float shortestDistance = float.MaxValue;

        foreach (var ip in highPriorityPoints)
        {
            NavMeshPath path = new();
            if (NavMesh.CalculatePath(
                GetSnappedPos(ip.position),
                GetSnappedPos(portal.transform.position),
                NavMesh.AllAreas, path)
            )
            {
                float dist = GetPathLength(path);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    bestPoint = ip;
                }
            }
        }
        result = bestPoint;
        return result;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0.0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }

    private Vector3 GetSnappedPos(Vector3 originalPos)
    {
        if (NavMesh.SamplePosition(originalPos, out NavMeshHit hit, 4.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        Debug.Log("Không snap được");
        return originalPos;
    }
}