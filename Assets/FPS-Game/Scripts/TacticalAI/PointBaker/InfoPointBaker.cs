using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class InfoPointBaker : PointBaker
{
    public float gridSize = 2.0f;
    public ZoneID selectedZone = ZoneID.None;

    Dictionary<ZoneID, List<InfoPoint>> infoPointsByZone = new();

    [ContextMenu("Generate InfoPoints for all zones")]
    public void GenerateInfoPoints()
    {
        if (ZoneManager.Instance == null) return;

        pointsDebug.Clear();
        infoPointsByZone.Clear();
        List<Zone> zones = ZoneManager.Instance.allZones;

        foreach (Zone zone in zones)
        {
            infoPointsByZone[zone.zoneData.zoneID] = new List<InfoPoint>();
            foreach (var col in zone.colliders)
            {
                Bounds bounds = col.bounds;
                for (float x = bounds.min.x; x <= bounds.max.x; x += gridSize)
                {
                    for (float z = bounds.min.z; z <= bounds.max.z; z += gridSize)
                    {
                        // Bắn Raycast từ trên xuống (Y max)
                        Vector3 rayStart = new(x, bounds.max.y, z);
                        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, bounds.size.y, ZoneManager.Instance.obstacleLayer))
                        {
                            // Kiểm tra điểm có nằm trên NavMesh không
                            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                            {
                                Vector3 targetPos = navHit.position + Vector3.up * ZoneManager.Instance.heightOffset;
                                pointsDebug.Add(targetPos);
                                infoPointsByZone[zone.zoneData.zoneID].Add(new()
                                {
                                    position = targetPos
                                });
                            }
                        }
                    }
                }
            }
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    [ContextMenu("Bake InfoPoint")]
    public void BakeInfoPoint()
    {
        if (ZoneManager.Instance == null) return;

        ClearOldPoints();
        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            foreach (InfoPoint infoPoint in infoPointsByZone[zone.zoneData.zoneID])
            {
                zone.zoneData.allPoints.Add(infoPoint);
                zone.zoneData.masterPoints.Add(infoPoint);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(zone.zoneData);
#endif

        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif

        Debug.Log("Bake InfoPoint Complete!");
    }

    void ClearOldPoints()
    {
        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            for (int i = zone.zoneData.allPoints.Count - 1; i >= 0; i--)
            {
                if (zone.zoneData.allPoints[i].type == PointType.Info)
                {
                    zone.zoneData.allPoints.RemoveAt(i);
                }
            }

            for (int i = zone.zoneData.masterPoints.Count - 1; i >= 0; i--)
            {
                if (zone.zoneData.masterPoints[i].type == PointType.Info)
                {
                    zone.zoneData.masterPoints.RemoveAt(i);
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(zone.zoneData);
#endif
        }
    }

    protected override void DrawGizmosSelected()
    {
        if (pointsDebug == null || pointsDebug.Count == 0) return;
        if (infoPointsByZone == null || infoPointsByZone.Count == 0) return;
        if (selectedZone == ZoneID.None) return;

        List<InfoPoint> targetInfoPoints = infoPointsByZone[selectedZone];
        foreach (InfoPoint infoPoint in targetInfoPoints)
        {
            Gizmos.color = pointColorGizmos;
            Gizmos.DrawSphere(infoPoint.position, pointSizeGizmos);
        }
    }
}