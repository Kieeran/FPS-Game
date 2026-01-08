using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PortalPointBaker : PointBaker
{
    protected override void ValidatePointInternal()
    {
        ValidatePointBase();
    }

    [ContextMenu("Create a new portal point GameObject")]
    public ZonePortal CreatePortalPointGO()
    {
        GameObject pointGO = CreatePointGOAt(Vector3.zero);
        ZonePortal portal = pointGO.AddComponent<ZonePortal>();
        return portal;
    }

    [ContextMenu("Bake PortalPoint")]
    public void BakePortalPoint()
    {
        if (ZoneManager.Instance == null || pointsHolder == null) return;

        List<Transform> pointsTransform = GetPointsTransform();
        if (pointsTransform.Count == 0)
        {
            Debug.LogWarning("Bake Warning: Danh sách pointsTransform đang trống!");
            return;
        }

        ClearOldPoints();

        SyncDebugPoints();
        ValidatePointInternal();

        List<PortalPoint> portalPoints = new();

        for (int i = pointsTransform.Count - 1; i >= 0; i--)
        {
            ZonePortal portal = pointsTransform[i].GetComponent<ZonePortal>();
            portalPoints.Add(new()
            {
                position = pointsTransform[i].position,
                portalName = portal.portalName,
                zoneDataA = portal.zoneA.zoneData,
                zoneDataB = portal.zoneB.zoneData
            });

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(pointsTransform[i].gameObject);
#else
            Destroy(pointsTransform[i].gameObject);
#endif

        }
        pointsTransform.Clear();

        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            foreach (PortalPoint portalPoint in portalPoints)
            {
                if (zone.zoneData.zoneID == portalPoint.zoneDataA.zoneID || zone.zoneData.zoneID == portalPoint.zoneDataB.zoneID)
                {
                    zone.zoneData.portals.Add(portalPoint);

#if UNITY_EDITOR
                    EditorUtility.SetDirty(zone.zoneData);
#endif
                }
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        // Lưu Scene để xác nhận các GameObject đã bị xóa vĩnh viễn
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(gameObject.scene);
        }
#endif

        Debug.Log("Bake PortalPoint Complete!");
    }

    void ClearOldPoints()
    {
        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            zone.zoneData.portals.Clear();

#if UNITY_EDITOR
            EditorUtility.SetDirty(zone.zoneData);
#endif

        }
    }

    [ContextMenu("Edit Points")]
    public void EditPoints()
    {
        if (ZoneManager.Instance == null) return;
        if (pointsHolder.childCount > 0) return; // Đã ở chế độ edit rồi

        int pointCount = 0;
        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            foreach (PortalPoint portalPoint in zone.zoneData.portals)
            {
                ZonePortal portal = CreatePortalPointGO();
                portal.name = portalPoint.portalName;
                portal.zoneA = ZoneManager.Instance.zoneCache[portalPoint.zoneDataA.zoneID];
                portal.zoneB = ZoneManager.Instance.zoneCache[portalPoint.zoneDataB.zoneID];
            }
        }

        Debug.Log($"Đã khôi phục {pointCount} điểm để chỉnh sửa.");
    }
}