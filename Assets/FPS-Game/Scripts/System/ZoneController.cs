using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZoneController : MonoBehaviour
{
    public List<Zone> allZones { get; private set; } = new();
    ZonesContainer zonesContainer;
    ZonePortalsContainer zonePortalsContainer;
    public void InitZones(ZonesContainer zonesContainer, ZonePortalsContainer zonePortalsContainer)
    {
        this.zonesContainer = zonesContainer;
        this.zonePortalsContainer = zonePortalsContainer;
        allZones = this.zonesContainer.GetZones();
    }

    public GameObject GetRandomTPAtBestZone()
    {
        Zone bestZone = GetBestZone();
        return bestZone.GetRandomTP().gameObject;
    }

    Zone GetBestZone()
    {
        if (allZones == null || allZones.Count <= 0)
        {
            Debug.Log("Unvalid zones list");
            return null;
        }

        Zone bestZone = allZones[0];
        foreach (Zone zone in allZones)
        {
            if (zone.GetCurrentWeight() > bestZone.GetCurrentWeight() && zone.TPoints.Count > 0)
            {
                bestZone = zone;
            }
        }
        bestZone.ResetWeight();
        Debug.Log($"Bot patrol to zone: {bestZone.gameObject.name}");

        return bestZone;
    }

    public (PointVisibilityData data, Zone zone) GetTargetForChase(Vector3 currentPos, Zone currentZone, TPointData data)
    {
        Zone targetZone = null;
        float bestMatch = -0.5f;

        Vector3 playerLookDir = (data.Rotation * Vector3.forward).normalized;

        foreach (var portal in currentZone.portals)
        {
            Vector3 dirToPortal = (portal.transform.position - data.Position).normalized;
            float dot = Vector3.Dot(playerLookDir, dirToPortal);

            if (dot > bestMatch)
            {
                bestMatch = dot;
                targetZone = portal.GetOtherZone(currentZone.zoneID);
            }
        }

        // Nếu không tìm thấy hàng xóm nào khả thi, mặc định tìm trong currentZone
        if (targetZone == null) targetZone = currentZone;

        return GetTarget(currentPos, currentZone, targetZone);
    }

    public (PointVisibilityData data, Zone zone) GetTargetForPatrol(Vector3 currentPos, Zone currentZone)
    {
        Zone targetZone = GetBestZone();
        return GetTarget(currentPos, currentZone, targetZone);
    }

    (PointVisibilityData data, Zone zone) GetTarget(Vector3 currentPos, Zone currentZone, Zone targetZone)
    {
        Vector3 point = GetFirstTPInZone(targetZone);
        ZonePortal portal = GetFinalPortalBeforeTarget(point, targetZone, currentPos, currentZone);

        PointVisibilityData data = portal.GetTargetNearestIPoint(targetZone.zoneID);

        Debug.Log($"Patrol to the nearest [{data.position}] and highest priority [{data.priority}]");
        Debug.Log($"Zone: {targetZone.zoneID}");

        return (data, targetZone);
    }

    public Vector3 GetFirstTPInZone(Zone zone)
    {
        return zone.TPoints[0].position;
    }

    private Zone GetZoneAtPosition(Vector3 pos)
    {
        foreach (Zone zone in allZones)
        {
            if (zone.IsPointInZone(pos))
            {
                return zone;
            }
        }
        return null;
    }

    public ZonePortal GetFinalPortalBeforeTarget(Vector3 targetPoint, Zone targetZone, Vector3 currentPos, Zone currentZone)
    {
        if (NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, 3.5f, NavMesh.AllAreas))
        {
            targetPoint = hit.position;
        }

        NavMeshPath path = new();
        if (NavMesh.CalculatePath(currentPos, targetPoint, NavMesh.AllAreas, path))
        {
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Đường đi không thông suốt");
                return null;
            }

            List<Zone> zoneSequence = new()
            {
                currentZone
            };

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Vector3 start = path.corners[i];
                Vector3 end = path.corners[i + 1];
                float segmentDistance = Vector3.Distance(start, end);

                // Chia nhỏ đoạn thẳng này: Cứ mỗi 1.0 đơn vị (mét) lấy 1 điểm để check
                float step = 1.0f;
                int iterations = Mathf.CeilToInt(segmentDistance / step);

                for (int j = 1; j <= iterations; j++)
                {
                    // Nội suy điểm nằm trên đoạn thẳng
                    float t = (float)j / iterations;
                    Vector3 checkPoint = Vector3.Lerp(start, end, t);

                    Zone zoneAtPoint = GetZoneAtPosition(checkPoint);

                    if (zoneAtPoint != null && !zoneSequence.Contains(zoneAtPoint))
                    {
                        zoneSequence.Add(zoneAtPoint);
                    }
                }
            }

            if (zoneSequence.Count >= 2 && zoneSequence.Last() == targetZone)
            {
                Zone penultimateZone = zoneSequence[zoneSequence.Count - 2];
                return penultimateZone.GetPortalTo(targetZone);
            }
        }
        return null;
    }
}