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

    public void CreatePointGOAt(Vector3 pos)
    {
        GameObject pointGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointGO.transform.SetParent(pointsHolder);
        pointGO.name = "Point#" + (pointsHolder.childCount - 1);
        pointGO.transform.localScale = Vector3.one * 0.2f;

        if (pos == Vector3.zero)
        {
            SceneView.lastActiveSceneView.MoveToView(pointGO.transform);
        }

        else
        {
            pointGO.transform.position = pos;
        }

        Selection.activeGameObject = pointGO;
        SyncDebugPoints();
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
}