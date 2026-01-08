using System;
using UnityEngine;

public class PlayerCollision : PlayerBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            Debug.Log("Pick up health");
            PlayerRoot.Events.InvokeOnCollectedHealthPickup();
            Destroy(other.gameObject);
        }

        // if (other.transform.parent.CompareTag("Zone"))
        // {
        //     Zone zone = other.GetComponentInParent<Zone>();
        //     PlayerRoot.PlayerUI.UpdateLocationText(zone.zoneID.ToString());
        //     PlayerRoot.CurrentZone = zone;

        //     Debug.Log($"Current zone: {zone.zoneID}");
        // }
        // Debug.Log($"Trigger log: {other.transform.parent.name}");
    }
}