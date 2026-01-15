using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType { Info, Tactical, Portal }

[System.Serializable]
public class InfoPoint
{
    public int pointID;
    public Vector3 position;
    public PointType type = PointType.Info;
    public int priority;
    public List<int> visibleIndices = new();

    [System.NonSerialized] public bool isChecked = false; // Dữ liệu Runtime
}

[System.Serializable]
public class TacticalPoint : InfoPoint
{
    public TacticalPoint() { type = PointType.Tactical; }
}

[System.Serializable]
public class PortalPoint : InfoPoint
{
    public PortalPoint() { type = PointType.Portal; }
    public string portalName;
    public ZoneData zoneDataA;
    public ZoneData zoneDataB;
    // public float traversalCost;

    public ZoneData GetOtherZone(ZoneData currentZone)
    {
        if (zoneDataA == currentZone) return zoneDataB;
        if (zoneDataB == currentZone) return zoneDataA;
        return null;
    }
}