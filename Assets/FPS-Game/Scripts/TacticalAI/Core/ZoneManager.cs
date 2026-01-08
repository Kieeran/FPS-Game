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

    [SerializeField] Transform zoneContainer;

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
}