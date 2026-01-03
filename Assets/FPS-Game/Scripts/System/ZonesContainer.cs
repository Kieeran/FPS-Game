using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZonesContainer : MonoBehaviour
{
    [SerializeField] List<Zone> zones;

    public float gizmoRadius = 0.5f;
    public string tpTag = "TacticalPoint";

    public List<Zone> GetZones() { return zones; }

    [ContextMenu("Bake All Zones")]
    public void BakeAllZones()
    {
        GameObject[] allTPs = GameObject.FindGameObjectsWithTag(tpTag);
        zones = GetComponentsInChildren<Zone>().ToList();

        foreach (var zone in zones)
        {
            // Truyền danh sách allTPs vào hàm Init của từng Zone
            zone.InitZone(allTPs);
        }
    }
}
