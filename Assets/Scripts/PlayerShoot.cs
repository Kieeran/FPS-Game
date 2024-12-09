using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject BulletSpawnPoint;
    [SerializeField] private GameObject hitScanSphere;
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    [SerializeField] private float FireCoolDown;
    private float CurrentCoolDown;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerAssetsInputs.shoot == true)
        {
            if (CurrentCoolDown <= 0)
            {
                CurrentCoolDown = FireCoolDown;
                Shoot();
            }
        }
        CurrentCoolDown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log(hit.point);

            GameObject sphere = Instantiate(hitScanSphere);
            sphere.transform.position = hit.point;
        }

        // Ray gunRay = new Ray(BulletSpawnPoint.transform.position, BulletSpawnPoint.transform.forward);

        // if (Physics.Raycast(gunRay, out RaycastHit hit))
        // {
        //     Debug.Log(hit.point);

        //     GameObject sphere = Instantiate(hitScanSphere);
        //     sphere.transform.position = hit.point;
        // }
    }
}