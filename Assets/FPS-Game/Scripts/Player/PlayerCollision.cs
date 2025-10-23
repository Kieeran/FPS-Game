using System;
using UnityEngine;

public class PlayerCollision : PlayerBehaviour
{
    public Action OnCollectedHealthPickup;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            Debug.Log("Pick up health");
            OnCollectedHealthPickup?.Invoke();
            Destroy(other.gameObject);
        }
    }
}