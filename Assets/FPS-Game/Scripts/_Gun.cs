using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using PlayerAssets;
using Unity.Netcode;

public class _Gun : NetworkBehaviour
{
    // public UnityEvent OnGunShoot;
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    public PlayerShoot playerShoot;

    private bool isPressed = false;

    public float FireCoolDown;

    // private bool isShoot;
    // private bool isAim;
    // private bool isReload;

    [SerializeField] private _ShootEffect shootEffect;

    // [SerializeField] private Vector3 aimPosition;
    // private Vector3 normalPosition;

    // public Image crossHair;

    public bool Automatic;

    private float CurrentCoolDown;

    // [SerializeField] private Magazine magazine;

    //private float delayTime = 1f;
    //private float counter = 0f;

    // [SerializeField] private GameObject bulletSpawnPoint;
    //[SerializeField]
    //private Transform orientation;
    // [SerializeField] private float fireRate;
    // [SerializeField] private float speed;

    public override void OnNetworkSpawn()
    {
        CurrentCoolDown = FireCoolDown;

        //normalPosition = transform.localPosition;

        // if (OnGunShoot == null)
        //     OnGunShoot = new UnityEvent();
    }

    private void OnShoot()
    {
        // if (magazine.IsMagazineEmpty()) return;
        // if (magazine.IsReloading()) return;

        if (Automatic)
        {
            if (playerAssetsInputs.shoot == true)
            {
                if (CurrentCoolDown <= 0f /*&& OnGunShoot != null*/)
                {
                    //OnGunShoot.Invoke();
                    CurrentCoolDown = FireCoolDown;
                    //ShootBullet();
                    playerShoot.Shoot();

                    shootEffect.ActiveShootEffect();
                }
            }
        }

        else
        {
            if (playerAssetsInputs.shoot == true && isPressed == false)
            {
                isPressed = true;

                if (CurrentCoolDown <= 0f /*&& OnGunShoot != null*/)
                {
                    //OnGunShoot.Invoke();
                    CurrentCoolDown = FireCoolDown;
                    //ShootBullet();
                    playerShoot.Shoot();

                    shootEffect.ActiveShootEffect();
                }
            }

            if (playerAssetsInputs.shoot == false) isPressed = false;
        }

        CurrentCoolDown -= Time.deltaTime;
    }

    private Vector3 forceDirection = Vector3.zero;

    // private void ShootBullet()
    // {
    //     Bullet bullet = BulletManager.Instance.GetBullet();
    //     bullet.transform.position = bulletSpawnPoint.transform.position;

    //     //Vector3 forceDirection = orientation.TransformDirection(orientation.forward) * speed;
    //     //Vector3 forceDirection = orientation.forward * speed;

    //     //Vector3 forceDirection = orientation.TransformDirection(orientation.forward) * speed;
    //     forceDirection = bulletSpawnPoint.transform.forward * speed;
    //     bullet.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
    //     bullet.StartCountingToDisappear();

    //     magazine.UpdateBulletsHud();
    // }

    // private float smooth = 10f;
    // float smoothRotation = 12f;
    // private void OnAim()
    // {
    //     if (Input.GetMouseButton(1))
    //     {
    //         transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * smooth);
    //         crossHair.gameObject.SetActive(false);

    //         transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * smoothRotation);

    //         //Debug.Log("Hold right click");
    //     }
    //     else if (Input.GetMouseButtonUp(1))
    //     {
    //         transform.localPosition = Vector3.Lerp(transform.localPosition, normalPosition, Time.deltaTime * smooth);
    //         crossHair.gameObject.SetActive(true);
    //         //Debug.Log("Release right click");
    //     }
    // }

    // private void OnReload()
    // {
    //     if (PlayerInput.Instance.GetPlayerAssetsInputs().reload == true)
    //     {
    //         magazine.Reload();
    //         PlayerInput.Instance.GetPlayerAssetsInputs().reload = false;
    //     }
    // }

    private void Update()
    {
        // isShoot = PlayerInput.Instance
        // isAim = 
        //isReload = PlayerInput.Instance.GetIsReloaded();

        if (IsOwner == false) return;

        OnShoot();
        //OnAim();
        //OnReload();
    }
}