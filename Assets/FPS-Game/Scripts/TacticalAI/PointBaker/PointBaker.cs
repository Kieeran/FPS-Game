using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PointBaker : MonoBehaviour
{
    public Transform poinsHolder;
    public List<Transform> pointsTransform = new();
    void OnValidate()
    {
        ValidateBase();
        OnValidateInternal();
    }

    protected virtual void OnValidateInternal() { }

    protected void ValidateBase()
    {
        if (ZoneManager.Instance == null) return;

        foreach (Transform pointTF in pointsTransform)
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
        }
    }
}