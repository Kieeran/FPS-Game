using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TacticalPointBaker : PointBaker
{
    protected override void ValidatePointInternal()
    {
        ValidatePointBase();
    }

    [ContextMenu("Bake TacticalPoint")]
    public void BakeTacticalPoint()
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

        int successCount = 0;
        int failCount = 0;

        for (int i = pointsTransform.Count - 1; i >= 0; i--)
        {
            Zone zone = ZoneManager.Instance.GetZoneAt(pointsTransform[i]);

            if (zone != null && zone.zoneData != null)
            {
                TacticalPoint tacticalPoint = new()
                {
                    position = pointsTransform[i].position
                };
                // zone.zoneData.tacticalPoints.Add(tacticalPoint);
                zone.zoneData.masterPoints.Add(tacticalPoint);
                successCount++;
            }
            else
            {
                failCount++;
                Debug.Log($"Điểm {pointsTransform[i].gameObject.name} ở vị trí local {pointsTransform[i].localPosition} không thuộc vùng nào");
            }

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(pointsTransform[i].gameObject);
#else
            Destroy(pointsTransform[i].gameObject);
#endif

        }
        pointsTransform.Clear();

#if UNITY_EDITOR
        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            zone.zoneData.UpdatePointID();
            EditorUtility.SetDirty(zone.zoneData);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        // Lưu Scene để xác nhận các GameObject đã bị xóa vĩnh viễn
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(gameObject.scene);
        }
#endif

        Debug.Log($"Bake TacticalPoint Complete! Thành công: {successCount}, Thất bại: {failCount}");
    }

    void ClearOldPoints()
    {
        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            // zone.zoneData.tacticalPoints.Clear();

            for (int i = zone.zoneData.masterPoints.Count - 1; i >= 0; i--)
            {
                if (zone.zoneData.masterPoints[i].type == PointType.Tactical)
                {
                    zone.zoneData.masterPoints.RemoveAt(i);
                }
            }

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
            foreach (TacticalPoint point in zone.zoneData.tacticalPoints)
            {
                CreatePointGOAt(point.position);
                pointCount++;
            }
        }

        Debug.Log($"Đã khôi phục {pointCount} điểm tactical để chỉnh sửa.");
    }
}