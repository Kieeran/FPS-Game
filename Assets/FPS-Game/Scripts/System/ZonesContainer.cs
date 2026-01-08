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
    public ZonePortalsContainer zonePortalsContainer;
    [SerializeField] List<Zone> zones;
    public float heightOffset = 2.84f;
    public float gizmoRadius = 0.5f;
    public LayerMask obstacleLayer;
    public string tpTag = "TacticalPoint";

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
