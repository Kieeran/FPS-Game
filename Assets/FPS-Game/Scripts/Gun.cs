using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using PlayerAssets;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    // public UnityEvent OnGunShoot;
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] PlayerInventory _playerInventory;
    [SerializeField] PlayerReload _playerReload;
    [SerializeField] PlayerShoot _playerShoot;
    [SerializeField] PlayerAim _playerAim;

    SupplyLoad _supplyLoad;

    private bool isPressed = false;

    public float FireCoolDown;

    // private bool isShoot;
    bool _isAim;
    bool _isUnAim;
    // private bool isReload;

    [SerializeField] private _ShootEffect shootEffect;

    [SerializeField] Vector3 _aimPos;
    [SerializeField] Vector3 _aimRot;
    [SerializeField] float _moveDuration;
    Vector3 _normalPos;
    Quaternion _normalRot;
    float _elapsedTime;

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

    void Start()
    {
        _supplyLoad = GetComponent<SupplyLoad>();
    }

    public override void OnNetworkSpawn()
    {
        CurrentCoolDown = FireCoolDown;

        _normalPos = transform.localPosition;
        _normalRot = transform.localRotation;

        // if (OnGunShoot == null)
        //     OnGunShoot = new UnityEvent();
    }

    void OnEnable()
    {
        _playerAim.OnAim += OnAim;
        _playerAim.OnUnAim += OnUnAim;
        _isAim = false;
        _isUnAim = false;
    }

    void OnDisable()
    {
        _playerAim.OnAim -= OnAim;
        _playerAim.OnUnAim -= OnUnAim;
    }

    void OnAim()
    {
        _isAim = true;
    }

    void OnUnAim()
    {
        _isUnAim = true;
    }

    private void Shoot()
    {
        if (_supplyLoad.IsMagazineEmpty()) return;
        if (_playerReload.GetIsReloading()) return;

        if (Automatic)
        {
            if (_playerAssetsInputs.shoot == true)
            {
                if (CurrentCoolDown <= 0f /*&& OnGunShoot != null*/)
                {
                    //OnGunShoot.Invoke();
                    CurrentCoolDown = FireCoolDown;
                    _playerInventory.UpdatecurrentMagazineAmmo();
                    _playerShoot.Shoot();

                    shootEffect.ActiveShootEffect();
                }
            }
        }

        else
        {
            if (_playerAssetsInputs.shoot == true && isPressed == false)
            {
                isPressed = true;

                if (CurrentCoolDown <= 0f /*&& OnGunShoot != null*/)
                {
                    //OnGunShoot.Invoke();
                    CurrentCoolDown = FireCoolDown;
                    _playerInventory.UpdatecurrentMagazineAmmo();
                    _playerShoot.Shoot();

                    shootEffect.ActiveShootEffect();
                }
            }

            if (_playerAssetsInputs.shoot == false) isPressed = false;
        }

        CurrentCoolDown -= Time.deltaTime;
    }

    void Aim()
    {
        if (_isAim == true)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / _moveDuration);

            transform.localPosition = Vector3.Lerp(_normalPos, _aimPos, t);
            transform.localRotation = Quaternion.Slerp(_normalRot, Quaternion.Euler(_aimRot), t);

            if (t >= 1f)
            {
                _isAim = false;
                _elapsedTime = 0;
            }
        }

        else if (_isUnAim == true)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / _moveDuration);

            transform.localPosition = Vector3.Lerp(_aimPos, _normalPos, t);
            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(_aimRot), _normalRot, t);

            if (t >= 1f)
            {
                _isUnAim = false;
                _elapsedTime = 0;
            }
        }
    }

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

        Shoot();
        Aim();
        //OnReload();
    }
}