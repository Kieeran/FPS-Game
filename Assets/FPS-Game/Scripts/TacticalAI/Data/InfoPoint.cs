using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType { Info, Tactical, Portal }

[System.Serializable]
public class InfoPoint
{
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
    public string portalName;
    public ZoneData zoneDataA;
    public ZoneData zoneDataB;
    public PortalPoint() { type = PointType.Portal; }
}