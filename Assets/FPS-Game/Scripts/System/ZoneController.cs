using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public List<Zone> allZones { get; private set; } = new();

    public void InitZones(ZonesContainer container)
    {
        allZones = container.GetZones();
    }

    public GameObject GetRandomTPAtBestZone()
    {
        if (allZones == null || allZones.Count <= 0)
        {
            Debug.Log("Unvalid zones list");
            return null;
        }

        Zone bestZone = allZones[0];
        foreach (Zone zone in allZones)
        {
            if (zone.GetCurrentWeight() > bestZone.GetCurrentWeight() && zone.TPoints.Count > 0)
            {
                bestZone = zone;
            }
        }

        bestZone.ResetWeight();
        Debug.Log($"Bot patrol to zone: {bestZone.gameObject.name}");

        return bestZone.GetRandomTP().gameObject;
    }
}
