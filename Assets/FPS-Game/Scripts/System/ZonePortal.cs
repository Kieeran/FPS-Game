using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePortal : MonoBehaviour
{
    public string portalName;
    public Zone zoneA;
    public Zone zoneB;

    // [Header("Basked Visibility Data")]
    // // Dữ liệu nhìn vào Zone B khi đứng ở mép Portal phía Zone A
    // public PointVisibilityData nearestIPointInA;
    // // Dữ liệu nhìn vào Zone A khi đứng ở mép Portal phía Zone B
    // public PointVisibilityData nearestIPointInB;

    // public Zone GetOtherZone(ZoneID currentZoneID)
    // {
    //     if (zoneA.zoneID == currentZoneID) return zoneB;
    //     if (zoneB.zoneID == currentZoneID) return zoneA;
    //     return null;
    // }

    // public PointVisibilityData GetTargetNearestIPoint(ZoneID targetZoneID)
    // {
    //     return targetZoneID == zoneA.zoneID ? nearestIPointInA : nearestIPointInB;
    // }

    // private void OnDrawGizmosSelected()
    // {
    //     if (zoneA != null && zoneB != null)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawCube(transform.position, Vector3.one);
    //     }
    // }
}