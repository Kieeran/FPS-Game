using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Unity.Netcode;

public class DamageGun : NetworkBehaviour
{
    public float headDamage;
    public float bodyDamage;
    public float BulletRange;
    public Transform playerCam;

    private void Start()
    {
        //PlayerCamera = Camera.main.transform;
    }

    public void Shoot()
    {
        if (IsOwner){
            Ray gunRay = new Ray(playerCam.position, playerCam.forward);
            RaycastHit hit;

            if (Physics.Raycast(gunRay, out hit, BulletRange))
            {
                //if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                //{
                //    enemy.GetDamage(damage);
                //    Debug.Log("Hit enemy");
                //}

                GameObject _gameObject = hit.collider.gameObject;

                if (_gameObject.transform.parent == null) return;

                if (_gameObject.transform.parent.TryGetComponent(out Dummy dummy))
                {
                    //Debug.Log("Dummy!!!");

                    if (dummy != null)
                    {
                        if (_gameObject.name == "Head")
                        {
                            dummy.GetDamage(150);
                            Debug.Log("Dummy head");
                        }

                        else if (_gameObject.name == "Body")
                        {
                            dummy.GetDamage(50);
                            Debug.Log("Dummy body");
                        }
                    }
                }

                if (_gameObject.name == "StartButton")
                {
                    if (_gameObject.transform.parent.TryGetComponent(out StartButton startButton))
                    {
                        startButton.StartPractice();
                    }
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
}