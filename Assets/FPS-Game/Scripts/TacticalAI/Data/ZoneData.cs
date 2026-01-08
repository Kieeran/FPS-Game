using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneID
{
    None, CT_Spawn, CT_Left, CT_Right, Stairs, Market, House, T_Spawn,
    Long_Cellar, Storage_Room, Stair_Room, Long, Streets, Tunnel
}

[CreateAssetMenu(fileName = "NewZoneData", menuName = "AI/Zone Data")]
public class ZoneData : ScriptableObject
{
    public ZoneID zoneID = ZoneID.None;
    public float baseWeight = 10f;     // Độ ưu tiên cố định
    public float growRate = 1f;        // Tốc độ tăng trọng số mỗi giây
    public Vector3 centerPos;

    [Header("Navigation Points (IP + TP)")]
    [SerializeReference]
    public List<InfoPoint> allPoints = new();

    [Header("Exits & Entrances")]
    public List<PortalPoint> portals = new();
}