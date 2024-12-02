using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public UnityEvent OnGunShoot;
    public float FireCoolDown;

    private Vector3 aimPosition;
    private Vector3 normalPosition;

    public Vector3 aimOffset;

    public Image crossHair;

    public bool Automatic;

    private float CurrentCoolDown;

    private Inventory inventory;

    //private float delayTime = 1f;
    //private float counter = 0f;

    //public GameObject muzzleFlash;

    [SerializeField]
    private GameObject bulletSpawnPoint;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float speed;

    private void Start()
    {
        //muzzleFlash.gameObject.SetActive(false);

        inventory = PlayerInput.Instance.GetInventory();

        CurrentCoolDown = FireCoolDown;

        normalPosition = transform.localPosition;

        if (OnGunShoot == null)
            OnGunShoot = new UnityEvent();
    }

    private void OnShoot()
    {
        Debug.Log(Inventory.currentMagazineAmmo);
        Debug.Log("(Gun.cs) isReloading" + Inventory.isReloading);
        if (IsOwner) {
            if (!PauseMenu.isPaused && !ChatManager.isChatting) {
                if (inventory.IsMagazineEmpty()) return;
                if (inventory.IsReloading()) return;

                if (Automatic)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (CurrentCoolDown <= 0f && OnGunShoot != null)
                        {
                            OnGunShoot.Invoke();
                            CurrentCoolDown = FireCoolDown;
                            ShootBullet();

                            //muzzleFlash.gameObject.SetActive(true);
                            Invoke("abc", FireCoolDown);

                            inventory.UpdateBulletsHud();
                        }
                    }
                }

                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (CurrentCoolDown <= 0f && OnGunShoot != null)
                        {
                            OnGunShoot.Invoke();
                            CurrentCoolDown = FireCoolDown;
                            ShootBullet();

                            //muzzleFlash.gameObject.SetActive(true);
                            Invoke("abc", FireCoolDown);

                            inventory.UpdateBulletsHud();
                        }
                    }
                }
            }

            CurrentCoolDown -= Time.deltaTime;
        }
    }

    void abc()
    {
        //muzzleFlash.gameObject.SetActive(false);
    }

    private Vector3 forceDirection = Vector3.zero;

    private void ShootBullet()
    {
        Bullet bullet = BulletManager.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.transform.position;

        //Vector3 forceDirection = orientation.TransformDirection(orientation.forward) * speed;
        //Vector3 forceDirection = orientation.forward * speed;

        //Vector3 forceDirection = orientation.TransformDirection(orientation.forward) * speed;
        forceDirection = bulletSpawnPoint.transform.forward * speed;
        bullet.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
        bullet.StartCountingToDisappear();
    }
    
    private void OnAim()
    {
        if (!PauseMenu.isPaused && !ChatManager.isChatting) {
            if (Input.GetMouseButtonDown(1))
            {
                transform.localPosition = aimPosition;
                crossHair.gameObject.SetActive(false);

                //Debug.Log("Hold right click");
            }
            else if (Input.GetMouseButtonUp(1))
            {
                transform.localPosition = normalPosition;
                crossHair.gameObject.SetActive(true);
                //Debug.Log("Release right click");
            }
        }
    }

    private void Update()
    {
        aimPosition = normalPosition - aimOffset;

        OnShoot();
        OnAim();

        //counter += Time.deltaTime * fireRate;
        //if (counter <= delayTime) return;
        //counter = 0f;
    }
}