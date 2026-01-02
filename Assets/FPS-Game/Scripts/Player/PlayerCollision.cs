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

        if (other.transform.parent.CompareTag("Area"))
        {
            PlayerRoot.PlayerUI.UpdateLocationText(other.transform.parent.gameObject.name);
        }
        // Debug.Log($"Trigger log: {other.transform.parent.name}");
    }
}