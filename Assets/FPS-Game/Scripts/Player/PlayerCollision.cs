using System;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Action OnCollectedHealthPickup;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            Debug.Log("AAAAACCC");
            OnCollectedHealthPickup?.Invoke();
            Destroy(other.gameObject);
        }
    }
}