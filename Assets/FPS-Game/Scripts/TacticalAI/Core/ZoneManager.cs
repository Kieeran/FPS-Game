using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }
    public List<Zone> allZones = new();
    public float heightOffset = 2.84f;

    public LayerMask obstacleLayer;
    [SerializeField] LayerMask zoneLayer;
    [SerializeField] Transform zoneContainer;

    public Dictionary<ZoneID, Zone> zoneCache;

    // Chạy cả Edit Mode & Play Mode
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
#if UNITY_EDITOR
        else if (Instance != this)
        {
            Debug.LogWarning(
                "Multiple ZoneManager detected in scene. Only one should exist.",
                this);
        }
#endif

        BuildZoneCache();
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    // Runtime-only logic
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnValidate()
    {
        if (zoneContainer != null)
        {
            allZones = zoneContainer.GetComponentsInChildren<Zone>(true).ToList();
        }
    }

    public Zone GetZoneByID(ZoneID zoneID)
    {
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData.zoneID == zoneID) return zone;
        }

        return null;
    }

    public Zone GetZoneAt(Transform pointTF, Vector3 pointPos)
    {
        if (pointTF == null)
        {
            if (allZones == null || allZones.Count == 0) return null;

            foreach (Zone zone in allZones)
            {
                foreach (var col in zone.colliders)
                {
                    // Kiểm tra nếu vị trí điểm xét nằm trong vùng của Collider
                    if (col.bounds.Contains(pointPos))
                    {
                        return zone;
                    }
                }
            }

            return null;
        }

        else
        {
            // Lấy bán kính từ SphereCollider của Object hoặc lấy scale
            float radius = 1.0f;
            if (pointTF.TryGetComponent<SphereCollider>(out var sphereCol))
            {
                radius = sphereCol.radius * pointTF.transform.lossyScale.x;
            }

            Collider[] hitColliders = Physics.OverlapSphere(pointTF.transform.position, radius, zoneLayer);
            if (hitColliders.Length == 0) return null;

            Zone zone = hitColliders[0].GetComponentInParent<Zone>();
            return zone;
        }
    }

    public void BuildZoneCache()
    {
        zoneCache = new();
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData != null && zone.zoneData.zoneID != ZoneID.None)
            {
                if (!zoneCache.ContainsKey(zone.zoneData.zoneID))
                    zoneCache.Add(zone.zoneData.zoneID, zone);
            }
        }
    }
}