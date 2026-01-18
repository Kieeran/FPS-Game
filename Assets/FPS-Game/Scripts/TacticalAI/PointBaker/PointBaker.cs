using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PointBaker : MonoBehaviour
{
    public Transform pointsHolder;
    public List<Vector3> pointsDebug = new();
    public float pointSizeGizmos = 0.2f;
    public Color pointColorGizmos;
    public Color invalidPointColorGizmos = Color.red;
    public ZoneID selectedZoneID = ZoneID.None;

    void OnValidate()
    {
        ValidatePointBase();
        ValidatePointInternal();
    }

    public List<Transform> GetPointsTransform()
    {
        List<Transform> pointsTransform = new();
        foreach (Transform t in pointsHolder)
        {
            pointsTransform.Add(t);
        }

        return pointsTransform;
    }

    protected virtual void ValidatePointInternal() { }

    protected void ValidatePointBase()
    {
        if (ZoneManager.Instance == null || pointsHolder == null) return;

        foreach (Transform pointTF in pointsHolder)
        {
            if (pointTF == null) continue;
            if (NavMesh.SamplePosition(pointTF.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Vector3 targetPos = hit.position + Vector3.up * ZoneManager.Instance.heightOffset;
                if (Vector3.Distance(pointTF.position, targetPos) > 0.01f)
                {
                    pointTF.position = targetPos;
                }
            }
            else
            {
                Debug.Log($"Điểm {pointTF.gameObject.name} ở vị trí {pointTF.position} khi snap xuống không nằm trên NavMeshSurface");
            }
        }
    }

    [ContextMenu("Create a new point GameObject")]
    public void CreatePointGO()
    {
        CreatePointGOAt(Vector3.zero);
    }

    public GameObject CreatePointGOAt(Vector3 pos)
    {
        GameObject pointGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointGO.transform.SetParent(pointsHolder);
        pointGO.name = "Point#" + (pointsHolder.childCount - 1);
        pointGO.transform.localScale = Vector3.one * 0.2f;

        if (pos == Vector3.zero)
        {
#if UNITY_EDITOR
            SceneView.lastActiveSceneView.MoveToView(pointGO.transform);
#endif
        }

        else
        {
            pointGO.transform.position = pos;
        }
#if UNITY_EDITOR
        Selection.activeGameObject = pointGO;
#endif
        SyncDebugPoints();

        return pointGO;
    }

    protected void SyncDebugPoints()
    {
        if (pointsHolder == null) return;

        // Nếu đang có GameObject, cập nhật pointsDebug theo vị trí Transform
        if (pointsHolder.childCount > 0)
        {
            pointsDebug.Clear();
            foreach (Transform child in pointsHolder)
            {
                if (child != null) pointsDebug.Add(child.position);
            }
        }
    }

    void DrawInfoPointsPriority()
    {
        if (ZoneManager.Instance == null) return;
        if (selectedZoneID == ZoneID.None) return;

        Zone targetZone = ZoneManager.Instance.GetZoneByID(selectedZoneID);
        foreach (InfoPoint point in targetZone.zoneData.masterPoints)
        {
#if UNITY_EDITOR
            // Hiển thị số Priority trên đầu điểm (Chỉ trong Scene View)
            Handles.Label(point.position + Vector3.up * 0.5f, point.priority.ToString());
#endif
        }
    }

    protected virtual void DrawGizmos() { }

    protected virtual void DrawGizmosSelected()
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

            bool isOnNavMesh = NavMesh.SamplePosition(pos, out NavMeshHit hit, 5f, NavMesh.AllAreas);
            if (isOnNavMesh)
            {
                Gizmos.color = pointColorGizmos;
                Gizmos.DrawSphere(hit.position, 0.5f);
                Gizmos.DrawLine(pos, hit.position);
            }
            else
            {
                Gizmos.color = invalidPointColorGizmos;
                Gizmos.DrawCube(pos, Vector3.one * 0.5f);
            }
        }
    }

    void OnDrawGizmos()
    {
        DrawGizmos();
    }

    void OnDrawGizmosSelected()
    {
        // DrawInfoPointsPriority();
        DrawGizmosSelected();
    }
}