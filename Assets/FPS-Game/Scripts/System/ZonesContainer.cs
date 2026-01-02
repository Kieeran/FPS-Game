using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZonesContainer : MonoBehaviour
{
    public float gizmoRadius = 0.5f;
    [SerializeField] List<Zone> zones;

    public List<Zone> GetZones() { return zones; }

    [ContextMenu("Bake All Zones")]
    public void BakeAllZones()
    {
        GameObject[] allTPs = GameObject.FindGameObjectsWithTag("TacticalPoint");
        zones = GetComponentsInChildren<Zone>().ToList();

        foreach (var zone in zones)
        {
            // Truyền danh sách allTPs vào hàm Init của từng Zone
            zone.InitZone(allTPs);
        }
    }
}
