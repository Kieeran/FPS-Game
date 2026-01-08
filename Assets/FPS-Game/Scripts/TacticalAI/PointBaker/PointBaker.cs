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

    [ContextMenu("Bake Visibility And Priority of InfoPoints")]
    public void BakeInfoPointVisibility()
    {
        if (ZoneManager.Instance == null) return;

        //         visibilityMatrix.Clear();

        //         // 1. Khởi tạo danh sách data
        //         for (int i = 0; i < generatedInfoPoints.Count; i++)
        //         {
        //             visibilityMatrix.Add(new PointVisibilityData
        //             {
        //                 position = generatedInfoPoints[i],
        //                 visibleIndices = new List<int>()
        //             });
        //         }

        //         // 2. Chiếu Raycast lẫn nhau (O(n^2))
        //         for (int i = 0; i < generatedInfoPoints.Count; i++)
        //         {
        //             Vector3 startPos = generatedInfoPoints[i];

        //             for (int j = 0; j < generatedInfoPoints.Count; j++)
        //             {
        //                 if (i == j) continue; // Không tự chiếu chính mình

        //                 Vector3 endPos = generatedInfoPoints[j];

        //                 // Bắn tia Linecast để kiểm tra vật cản
        //                 if (!Physics.Linecast(startPos, endPos, zonesContainer.obstacleLayer))
        //                 {
        //                     // Nếu không có vật cản, thêm index j vào danh sách nhìn thấy của i
        //                     visibilityMatrix[i].visibleIndices.Add(j);
        //                 }
        //             }

        //             // 3. Gán Priority dựa trên số lượng điểm nhìn thấy
        //             visibilityMatrix[i].priority = visibilityMatrix[i].visibleIndices.Count;
        //         }

        // #if UNITY_EDITOR
        //         UnityEditor.EditorUtility.SetDirty(this);
        // #endif

        //         Debug.Log($"Bake hoàn tất cho {gameObject.name}. Điểm cao nhất nhìn thấy được {visibilityMatrix.Max(x => x.priority)} điểm khác.");
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
            SceneView.lastActiveSceneView.MoveToView(pointGO.transform);
        }

        else
        {
            pointGO.transform.position = pos;
        }

        Selection.activeGameObject = pointGO;
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
        DrawGizmosSelected();
    }
}