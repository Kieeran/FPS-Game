using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float headDamage;
    public float bodyDamage;
    public float BulletRange;
    public Transform PlayerCamera;

    private void Start()
    {
        //PlayerCamera = Camera.main.transform;
    }

    public void Shoot()
    {
        Ray gunRay = new Ray(PlayerCamera.position, PlayerCamera.forward);
        RaycastHit hit;

        //LayerMask layerMask = ~LayerMask.GetMask("Wall");

        if (Physics.Raycast(gunRay, out hit, BulletRange))
        {
            //if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
            //{
            //    enemy.GetDamage(damage);
            //    Debug.Log("Hit enemy");
            //}

            Debug.Log(hit.collider.name);

            //if (hit.collider.transform.name == "Cube")
            //    Debug.Log(hit.collider.transform.position);

            Collider collider = hit.collider;

            if (collider.transform.parent.TryGetComponent(out Dummy dummy))
            {
                if (dummy != null)
                {
                    if (collider.gameObject.name == "Head")
                    {
                        dummy.GetDamage(150);
                        Debug.Log("Dummy head");
                    }

                    else if (collider.gameObject.name == "Body")
                    {
                        dummy.GetDamage(50);
                        Debug.Log("Dummy body");
                    }
                }
            }

            if (collider.transform.parent.TryGetComponent(out StartButton startButton))
            {
                if (startButton.GetIsStartPractice() != true) startButton.StartPractice();
            }

            //if (hit.collider.gameObject.TryGetComponent(out Entity enemy))
            //{
            //    enemy.GetDamage();
            //    enemy.CreateBulletHole(hit);
            //}

            //float positionMultiplier = 1f;
            //Vector3 spawnPosition = new Vector3(
            //    hit.point.x - gunRay.direction.x * positionMultiplier,
            //    hit.point.y - gunRay.direction.y * positionMultiplier,
            //    hit.point.z - gunRay.direction.z * positionMultiplier
            //);

            //BulletHole bulletHole = BulletHoleManager.Instance.GetBulletHole();
            //bulletHole.transform.position = hit.point;
            //bulletHole.transform.SetParent(hit.collider.transform);

            //Quaternion bulletRotation = Quaternion.LookRotation(gunRay.direction);
            //bulletHole.transform.rotation = bulletRotation;
        }
    }
}