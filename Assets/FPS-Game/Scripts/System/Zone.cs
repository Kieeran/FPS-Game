using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public List<Transform> TPoints = new();
    Collider[] colliders;
    ZonesContainer container;

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