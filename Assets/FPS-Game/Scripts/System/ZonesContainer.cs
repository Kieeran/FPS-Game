using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ZoneID
{
    None, CT_Spawn, CT_Left, CT_Right, Stairs, Market, House, T_Spawn,
    Long_Cellar, Storage_Room, Stair_Room, Long, Streets, Tunnel
}

public class ZonesContainer : MonoBehaviour
{
    [SerializeField] List<Zone> zones;
    public float heightOffset = 2.84f;
    public float gizmoRadius = 0.5f;
    public LayerMask obstacleLayer;
    public string tpTag = "TacticalPoint";

    // Dictionary lưu trữ: ZoneID hiện tại -> Danh sách các Portal dẫn đi các Zone khác
    public Dictionary<ZoneID, List<ZonePortal>> zoneAdjacencyMap = new();

    void Awake()
    {
        RebuildAdjacencyMap();
    }

    void RebuildAdjacencyMap()
    {
        zoneAdjacencyMap.Clear();

        foreach (var zone in zones)
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

    [ContextMenu("Scan All Portals")]
    public void ScanAllPortals()
    {
        zoneAdjacencyMap.Clear();
        ZonePortal[] allPortals = FindObjectsOfType<ZonePortal>();

        foreach (var portal in allPortals)
        {
            // Đăng ký cho Zone A
            RegisterConnection(portal.zoneA.zoneID, portal);
            // Đăng ký cho Zone B (để Portal có tính 2 chiều)
            RegisterConnection(portal.zoneB.zoneID, portal);
        }

        foreach (var zone in zones)
        {
            zone.portals.Clear();
            zone.portals = zoneAdjacencyMap[zone.zoneID];
        }

        Debug.Log($"Đã cập nhật bản đồ giao thông với {allPortals.Length} cổng kết nối.");
    }

    private void RegisterConnection(ZoneID zoneID, ZonePortal portal)
    {
        if (!zoneAdjacencyMap.ContainsKey(zoneID))
        {
            zoneAdjacencyMap[zoneID] = new List<ZonePortal>();
        }
        zoneAdjacencyMap[zoneID].Add(portal);
    }

    public List<Zone> GetZones() { return zones; }

    [ContextMenu("Bake All Zones")]
    public void BakeAllZones()
    {
        GameObject[] allTPs = GameObject.FindGameObjectsWithTag(tpTag);
        zones = GetComponentsInChildren<Zone>().ToList();

        int allInfoPointsCount = 0;
        foreach (var zone in zones)
        {
            // Truyền danh sách allTPs vào hàm Init của từng Zone
            zone.InitZone(allTPs);

            zone.GenerateInfoPoints();
            allInfoPointsCount += zone.generatedInfoPoints.Count;

            zone.BakeVisibility();
        }

        Debug.Log($"Đã tạo {allInfoPointsCount} InfoPoints cho tất cả các zone");
    }

    [ContextMenu("Clear All InfoPoints")]
    public void ClearAllInfoPoints()
    {
        foreach (var zone in zones)
        {
            zone.ClearInfoPoints();
        }
    }
}
