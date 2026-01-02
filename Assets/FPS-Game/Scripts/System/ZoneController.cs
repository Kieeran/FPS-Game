using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public List<Zone> allZones { get; private set; } = new();
    [SerializeField] ZonesContainer zonesContainer;

    public void InitZones(ZonesContainer container)
    {
        zonesContainer = container;
        allZones = container.GetZones();
    }
}
