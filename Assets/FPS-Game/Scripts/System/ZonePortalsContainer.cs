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

    void OnValidate()
    {
        zonePortals = GetComponentsInChildren<ZonePortal>().ToList();
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