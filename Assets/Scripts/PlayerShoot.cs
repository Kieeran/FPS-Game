using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject BulletSpawnPoint;
    [SerializeField] private GameObject hitScanSphere;
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerAssetsInputs.shoot == true) Shoot();
    }

    void Shoot()
    {
        Ray gunRay = new Ray(BulletSpawnPoint.transform.position, BulletSpawnPoint.transform.forward);

        if (Physics.Raycast(gunRay, out RaycastHit hit))
        {
            Debug.Log(hit.collider.name);
        }
    }
}