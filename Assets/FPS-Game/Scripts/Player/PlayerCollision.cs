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
    }
}