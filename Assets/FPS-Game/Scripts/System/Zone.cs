using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PointVisibilityData
{
    public Vector3 position;
    public int priority;           // Số lượng điểm nhìn thấy được
    public List<int> visibleIndices; // Danh sách index của các điểm nhìn thấy được
}

public class Zone : MonoBehaviour
{
    public ZoneID zoneID = ZoneID.None;
    public List<Transform> TPoints = new();
    public float baseWeight = 10f;     // Độ ưu tiên cố định
    public float growRate = 1f;        // Tốc độ tăng trọng số mỗi giây

    Collider[] colliders;
    ZonesContainer zonesContainer;
    ZonePortalsContainer zonePortalsContainer;
    float lastVisitedTime;     // Thời điểm cuối cùng được kiểm tra
    public float gridSize = 2.0f;
    public List<Vector3> generatedInfoPoints = new();

    public List<PointVisibilityData> visibilityMatrix = new();

    public List<ZonePortal> portals = new();

    [ContextMenu("Bake Visibility And Priority")]
    public void BakeVisibility()
    {
        if (generatedInfoPoints == null || generatedInfoPoints.Count <= 0) return;

        visibilityMatrix.Clear();

        // 1. Khởi tạo danh sách data
        for (int i = 0; i < generatedInfoPoints.Count; i++)
        {
            visibilityMatrix.Add(new PointVisibilityData
            {
                position = generatedInfoPoints[i],
                visibleIndices = new List<int>()
            });
        }

        // 2. Chiếu Raycast lẫn nhau (O(n^2))
        for (int i = 0; i < generatedInfoPoints.Count; i++)
        {
            Vector3 startPos = generatedInfoPoints[i];

            for (int j = 0; j < generatedInfoPoints.Count; j++)
            {
                if (i == j) continue; // Không tự chiếu chính mình

                Vector3 endPos = generatedInfoPoints[j];

                // Bắn tia Linecast để kiểm tra vật cản
                if (!Physics.Linecast(startPos, endPos, zonesContainer.obstacleLayer))
                {
                    // Nếu không có vật cản, thêm index j vào danh sách nhìn thấy của i
                    visibilityMatrix[i].visibleIndices.Add(j);
                }
            }

            // 3. Gán Priority dựa trên số lượng điểm nhìn thấy
            visibilityMatrix[i].priority = visibilityMatrix[i].visibleIndices.Count;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        Debug.Log($"Bake hoàn tất cho {gameObject.name}. Điểm cao nhất nhìn thấy được {visibilityMatrix.Max(x => x.priority)} điểm khác.");
    }

    [ContextMenu("Generate InfoPoints for this Zone")]
    public void GenerateInfoPoints()
    {
        // 1. Xóa các điểm cũ trước khi tạo mới
        ClearInfoPoints();

        foreach (var col in colliders)
        {
            Bounds bounds = col.bounds;

            // 2. Vòng lặp quét theo trục X và Z
            for (float x = bounds.min.x; x <= bounds.max.x; x += gridSize)
            {
                for (float z = bounds.min.z; z <= bounds.max.z; z += gridSize)
                {
                    // 3. Bắn Raycast từ trên xuống (Y max)
                    Vector3 rayStart = new(x, bounds.max.y, z);
                    if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, bounds.size.y, zonesContainer.obstacleLayer))
                    {
                        // 4. Kiểm tra điểm có nằm trên NavMesh không
                        if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                        {
                            generatedInfoPoints.Add(navHit.position + Vector3.up * zonesContainer.heightOffset);
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        Debug.Log($"Đã tạo {generatedInfoPoints.Count} InfoPoints cho Zone: {gameObject.name}");
    }

    public void ClearInfoPoints()
    {
        generatedInfoPoints.Clear();
    }

    public ZonePortal GetPortalTo(Zone targetZone)
    {
        List<ZonePortal> zonePortals = zonePortalsContainer.zoneAdjacencyMap[zoneID];
        foreach (var portal in zonePortals)
        {
            Zone zone = portal.GetOtherZone(zoneID);
            if (zone == targetZone) return portal;
        }
        return null;
    }

    public bool IsPointInZone(Vector3 pos)
    {
        foreach (var col in colliders)
        {
            // Kiểm tra xem điểm có nằm trong Bounds của Collider không
            if (col.bounds.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        // Khởi tạo ngẫu nhiên để Bot không đi trùng nhau lúc đầu
        lastVisitedTime = Time.time - Random.Range(0, 60f);
    }

    public float GetCurrentWeight()
    {
        // Trọng số hiện tại = Trọng số gốc + (thời gian trôi qua * tốc độ tăng)
        return baseWeight + (Time.time - lastVisitedTime) * growRate;
    }

    public void ResetWeight()
    {
        lastVisitedTime = Time.time;
        Debug.Log($"Zone has been reset: {gameObject.name}");
    }

    public Transform GetRandomTP()
    {
        if (TPoints.Count == 0) return null;
        return TPoints[Random.Range(0, TPoints.Count)];
    }

    void OnValidate()
    {
        colliders = GetComponentsInChildren<Collider>();
        zonesContainer = GetComponentInParent<ZonesContainer>();
        zonePortalsContainer = zonesContainer.zonePortalsContainer;
    }

    [ContextMenu("Bake Zone Points")]
    public void BakeZonePoints()
    {
        GameObject[] allTPs = GameObject.FindGameObjectsWithTag(zonesContainer.tpTag);
        InitZone(allTPs);
    }

    public void InitZone(GameObject[] allTPs)
    {
        if (colliders == null || colliders.Length <= 0) return;

        TPoints.Clear();
        foreach (GameObject tp in allTPs)
        {
            foreach (var col in colliders)
            {
                // Kiểm tra nếu vị trí TP nằm trong vùng của Collider
                if (col.bounds.Contains(tp.transform.position))
                {
                    if (!TPoints.Contains(tp.transform))
                    {
                        TPoints.Add(tp.transform);
                    }
                    break; // TP đã thuộc Zone này, không cần kiểm tra các Box khác cùng Zone
                }
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject != gameObject &&
            UnityEditor.Selection.activeGameObject != transform.parent.gameObject)
        {
            return;
        }

        // if (TPoints != null && TPoints.Count > 0)
        // {
        //     foreach (Transform tp in TPoints)
        //     {
        //         if (tp == null) continue;
        //         if (NavMesh.SamplePosition(tp.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        //         {
        //             Gizmos.color = Color.green;
        //             Gizmos.DrawSphere(hit.position, container.gizmoRadius);
        //             Gizmos.DrawLine(tp.position, hit.position);
        //         }
        //     }
        // }

        if (generatedInfoPoints != null && generatedInfoPoints.Count > 0)
        {
            for (int i = 0; i < generatedInfoPoints.Count; i++)
            {
                // Vẽ điểm, màu đậm nhạt tùy theo Priority
                float intensity = visibilityMatrix.Count > 0 ? (float)visibilityMatrix[i].priority / generatedInfoPoints.Count : 0;
                Gizmos.color = Color.Lerp(Color.red, Color.green, intensity);
                Gizmos.DrawSphere(generatedInfoPoints[i], 0.2f);

                // Hiển thị số Priority trên đầu điểm (Chỉ trong Scene View)
                UnityEditor.Handles.Label(generatedInfoPoints[i] + Vector3.up * 0.5f, visibilityMatrix.Count > i ? visibilityMatrix[i].priority.ToString() : "");
            }
        }

#endif
    }
}