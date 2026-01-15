using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneID
{
    None, CT_Spawn, CT_Left, CT_Right, Stairs, Market, House, T_Spawn,
    Long_Cellar, Storage_Room, Stair_Room, Long, Streets, Tunnel
}

[System.Serializable]
public class PortalConnection
{
    public int portalAID;
    public int portalBID;

    [InspectorReadOnly] public PortalPoint portalA;
    [InspectorReadOnly] public PortalPoint portalB;
    public float traversalCost; // Quãng đường NavMesh giữa 2 portal trong cùng 1 zone

    public void Resolve(List<InfoPoint> master)
    {
        portalA = master[portalAID] as PortalPoint;
        portalB = master[portalBID] as PortalPoint;
    }
}

[CreateAssetMenu(fileName = "NewZoneData", menuName = "AI/Zone Data")]
public class ZoneData : ScriptableObject, ISerializationCallbackReceiver
{
    public ZoneID zoneID = ZoneID.None;
    public float baseWeight = 10f;     // Độ ưu tiên cố định
    public float growRate = 1f;        // Tốc độ tăng trọng số mỗi giây
    public Vector3 centerPos;

    [Header("Master Data")]
    [SerializeReference]
    public List<InfoPoint> masterPoints = new();

    [InspectorReadOnly] public List<InfoPoint> infoPoints = new();
    [InspectorReadOnly] public List<TacticalPoint> tacticalPoints = new();
    [InspectorReadOnly] public List<PortalPoint> portals = new();

    [Header("Portal Connection")]
    public List<PortalConnection> internalPaths = new();

    public void UpdatePointID()
    {
        for (int i = 0; i < masterPoints.Count; i++)
        {
            masterPoints[i].pointID = i;
        }
        SyncReferences();
    }

    public void ResetIsChecked()
    {
        for (int i = 0; i < masterPoints.Count; i++)
        {
            masterPoints[i].isChecked = false;
        }
        SyncReferences();
    }

    public void OnAfterDeserialize()
    {
        SyncReferences();
    }

    public void OnBeforeSerialize() { }

    // Chạy mỗi khi thay đổi giá trị trong Edit Mode
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Delay một chút để tránh xung đột với quá trình Serialization của Unity
        UnityEditor.EditorApplication.delayCall += SyncReferences;
    }
#endif

    public void SyncReferences()
    {
        if (masterPoints == null) return;

        infoPoints.Clear();
        tacticalPoints.Clear();
        portals.Clear();

        foreach (var p in masterPoints)
        {
            if (p == null) continue;

            if (p is PortalPoint pp) portals.Add(pp);
            else if (p is TacticalPoint tp) tacticalPoints.Add(tp);
            else infoPoints.Add(p);
        }

        // Cập nhật lại các liên kết đường đi nội bộ
        if (internalPaths != null)
        {
            foreach (var path in internalPaths)
            {
                // Giả sử bạn dùng ID hoặc Index để Resolve
                path.Resolve(masterPoints);
            }
        }
    }
}