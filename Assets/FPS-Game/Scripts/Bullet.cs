using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void StartCountingToDisappear()
    {
        Invoke("ReturnSelf", 2f);
    }

    private void ReturnSelf()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        TrailRenderer trail = GetComponent<TrailRenderer>();
        //trail.enabled = false;
        //trail.enabled = true;
        trail.Clear();
        BulletManager.Instance.ReturnBullet(this);
    }
}