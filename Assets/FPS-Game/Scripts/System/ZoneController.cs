using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public List<Zone> allZones { get; private set; } = new();
    [SerializeField] Transform zonesContainer;

    public void InitZones(Transform tf)
    {
        zonesContainer = tf;

        foreach (Transform t in zonesContainer)
        {
            if (t.TryGetComponent<Zone>(out var zone))
            {
                allZones.Add(zone);
            }
        }
    }
}
