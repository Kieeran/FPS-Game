using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public List<Transform> TPoints = new();
    public float baseWeight = 10f;     // Độ ưu tiên cố định
    public float growRate = 1f;        // Tốc độ tăng trọng số mỗi giây

    Collider[] colliders;
    ZonesContainer container;
    float lastVisitedTime;     // Thời điểm cuối cùng được kiểm tra

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
        container = GetComponentInParent<ZonesContainer>();
    }

    [ContextMenu("Bake Zone Points")]
    public void BakeZonePoints()
    {
        GameObject[] allTPs = GameObject.FindGameObjectsWithTag(container.tpTag);
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
        if (TPoints == null || TPoints.Count <= 0) return;

        foreach (Transform tp in TPoints)
        {
            if (tp == null) continue;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(tp.position, container.gizmoRadius);
        }
#endif
    }
}