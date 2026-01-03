using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePortal : MonoBehaviour
{
    [Header("Connections")]
    public string portalName;
    public Zone zoneA;
    public Zone zoneB;

    public Zone GetOtherZone(ZoneID currentZoneID)
    {
        if (zoneA.zoneID == currentZoneID) return zoneB;
        if (zoneB.zoneID == currentZoneID) return zoneA;
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (zoneA != null && zoneB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }
}