using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CenterPointBaker : PointBaker
{
    protected override void ValidatePointInternal()
    {
        ValidatePointBase();
    }

    [ContextMenu("Bake CenterPoint")]
    public void BakeCenterPoint()
    {
        if (ZoneManager.Instance == null || pointsHolder == null) return;

        List<Transform> pointsTransform = GetPointsTransform();
        if (pointsTransform.Count == 0)
        {
            Debug.LogWarning("Bake Warning: Danh sách pointsTransform đang trống!");
            return;
        }

        SyncDebugPoints();
        ValidatePointInternal();

        int successCount = 0;
        int failCount = 0;

        for (int i = pointsTransform.Count - 1; i >= 0; i--)
        {
            Zone zone = ZoneManager.Instance.GetZoneAt(pointsTransform[i], Vector3.zero);

            if (zone != null && zone.zoneData != null)
            {
                zone.zoneData.centerPos = pointsTransform[i].position;

#if UNITY_EDITOR
                EditorUtility.SetDirty(zone.zoneData);
#endif

                successCount++;
            }
            else
            {
                failCount++;
            }

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(pointsTransform[i].gameObject);
#else
            Destroy(pointsTransform[i].gameObject);
#endif

        }
        pointsTransform.Clear();

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

        Debug.Log($"Bake CenterPoint Complete! Thành công: {successCount}, Thất bại: {failCount}");
    }

    [ContextMenu("Edit Points")]
    public void EditPoints()
    {
        if (ZoneManager.Instance == null) return;
        if (pointsHolder.childCount > 0) return; // Đã ở chế độ edit rồi

        foreach (Zone zone in ZoneManager.Instance.allZones)
        {
            if (zone.zoneData.centerPos != Vector3.zero)
                CreatePointGOAt(zone.zoneData.centerPos);
        }

        Debug.Log($"Đã khôi phục {pointsHolder.childCount} điểm để chỉnh sửa.");
    }

    private void OnDrawGizmosSelected()
    {
        if (pointsHolder != null && pointsHolder.childCount > 0)
        {
            SyncDebugPoints();
        }

        if (pointsDebug == null || pointsDebug.Count == 0) return;

        foreach (Vector3 pos in pointsDebug)
        {
            Gizmos.color = pointColorGizmos;
            Gizmos.DrawSphere(pos, pointSizeGizmos);

            bool isOnNavMesh = NavMesh.SamplePosition(pos, out NavMeshHit hit, 10f, NavMesh.AllAreas);
            if (isOnNavMesh)
            {
                Gizmos.color = pointColorGizmos;
                Gizmos.DrawSphere(hit.position, 0.5f);
                Gizmos.DrawLine(pos, hit.position);
            }
            else
            {
                Gizmos.color = invalidPointColorGizmos;
                Gizmos.DrawCube(pos, Vector3.one * 15f);
            }
        }
    }
}