using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Gun : PlayerBehaviour
{
    public SwayAndBob SwayAndBob { get; private set; }

    SupplyLoad _supplyLoad;
    private bool isPressed = false;
    public float FireCoolDown;

    [SerializeField] _ShootEffect shootEffect;
    [SerializeField] float _aimFOV;

    [SerializeField] float _spreadAngle;

    [SerializeField] AudioSource gunSound;

    // public Image crossHair;

    public bool Automatic;

    private float CurrentCoolDown;

    private float nextFireTime;

    // [SerializeField] private Magazine magazine;

    //private float delayTime = 1f;
    //private float counter = 0f;

    // [SerializeField] private GameObject bulletSpawnPoint;
    //[SerializeField]
    //private Transform orientation;
    // [SerializeField] private float fireRate;
    // [SerializeField] private float speed;

    [Header("Damage")]
    [SerializeField] float _headDamage;
    [SerializeField] float _torsoDamage;
    [SerializeField] float _legDamage;

    [Header("Gun type")]
    [SerializeField] GunType _gunType;
    [SerializeField] float _aimTransitionDuration;

    public float HeadDamage { get; private set; }
    public float TorsoDamage { get; private set; }
    public float LegDamage { get; private set; }

    public float GetAimFOV() { return _aimFOV; }

    public override void InitializeAwake()
    {
        base.InitializeAwake();
        SwayAndBob = GetComponent<SwayAndBob>();

        HeadDamage = _headDamage;
        TorsoDamage = _torsoDamage;
        LegDamage = _legDamage;
    }

    public override void InitializeStart()
    {
        base.InitializeStart();
        _supplyLoad = GetComponent<SupplyLoad>();
    }

    public override void OnNetworkSpawn()
    {
        CurrentCoolDown = FireCoolDown;

        // if (OnGunShoot == null)
        //     OnGunShoot = new UnityEvent();
    }

    void OnEnable()
    {
        PlayerRoot.Events.OnAimStateChanged += HandleAimStateChanged;

        gunSound.spatialBlend = 1f;
        gunSound.maxDistance = 100f;
    }

    void OnDisable()
    {
        PlayerRoot.Events.OnAimStateChanged -= HandleAimStateChanged;
    }

    void HandleAimStateChanged(bool isAim)
    {
        if (isAim)
        {
            if (PlayerRoot.WeaponHolder.WeaponPoseLocalSOs[_gunType].TryGetPose(PlayerWeaponPose.Aim, out var data))
            {
                StartCoroutine(TransitionAimState(data.Position, data.EulerRotation));
            }
        }
        else
        {
            if (PlayerRoot.WeaponHolder.WeaponPoseLocalSOs[_gunType].TryGetPose(PlayerWeaponPose.Idle, out var data))
            {
                StartCoroutine(TransitionAimState(data.Position, data.EulerRotation));
            }
        }
    }

    private void Shoot()
    {
        if (_supplyLoad.IsMagazineEmpty()) return;
        if (PlayerRoot.PlayerReload.GetIsReloading()) return;

        if (Automatic)
        {
            if (PlayerRoot.PlayerAssetsInputs.shoot == true)
            {
                if (CurrentCoolDown <= 0f /*&& OnGunShoot != null*/)
                {
                    //OnGunShoot.Invoke();
                    CurrentCoolDown = FireCoolDown;
                    PlayerRoot.PlayerInventory.UpdatecurrentMagazineAmmo();
                    PlayerRoot.PlayerShoot.Shoot(_spreadAngle, _gunType);

                    // shootEffect.ActiveShootEffect();
                    shootEffect.ActiveShootEffect();

                    if (gameObject.tag == "AK47")
                    {
                        if (Time.time >= nextFireTime)
                        {
                            // ak47Sound.Stop();
                            PlayGunAudio_ServerRpc(transform.position);
                            nextFireTime = Time.time + FireCoolDown;
                        }
                        // ak47Sound.Stop();
                    }
                }
            }
        }

        else
        {
            if (PlayerRoot.PlayerAssetsInputs.shoot == true && isPressed == false)
            {
                isPressed = true;

                if (CurrentCoolDown <= 0f /*&& OnGunShoot != null*/)
                {
                    //OnGunShoot.Invoke();
                    CurrentCoolDown = FireCoolDown;
                    PlayerRoot.PlayerInventory.UpdatecurrentMagazineAmmo();
                    PlayerRoot.PlayerShoot.Shoot(_spreadAngle, _gunType);

                    // shootEffect.ActiveShootEffect();
                    shootEffect.ActiveShootEffect();

                    if (gameObject.tag == "Sniper")
                    {
                        if (Time.time >= nextFireTime)
                        {
                            StopGunAudio_ServerRpc(transform.position);
                            PlayGunAudio_ServerRpc(transform.position);
                            // nextFireTime = Time.time + FireCoolDown;
                        }
                    }

                    if (gameObject.tag == "Pistol")
                    {
                        if (Time.time >= nextFireTime)
                        {
                            StopGunAudio_ServerRpc(transform.position);
                            PlayGunAudio_ServerRpc(transform.position);
                            // nextFireTime = Time.time + FireCoolDown;
                        }
                    }
                }
            }

            if (PlayerRoot.PlayerAssetsInputs.shoot == false) isPressed = false;
        }

        CurrentCoolDown -= Time.deltaTime;
    }

    public IEnumerator TransitionAimState(Vector3 targetPos, Vector3 targetEulerRot)
    {
        Vector3 originPos = transform.localPosition;
        Vector3 originEulerRot = transform.localEulerAngles;

        if (originPos == targetPos && originEulerRot == targetEulerRot)
            yield break;

        SwayAndBob.enabled = false;

        Quaternion originRot = Quaternion.Euler(originEulerRot);
        Quaternion targetRot = Quaternion.Euler(targetEulerRot);

        float elapsedTime = 0f;

        while (elapsedTime < _aimTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _aimTransitionDuration);

            Vector3 newPos = Vector3.Lerp(originPos, targetPos, t);
            Quaternion newRot = Quaternion.Slerp(originRot, targetRot, t);

            transform.SetLocalPositionAndRotation(newPos, newRot);

            yield return null;
        }

        transform.localPosition = targetPos;
        transform.localEulerAngles = targetEulerRot;

        SwayAndBob.enabled = true;
        SwayAndBob.AimPositionOffset = transform.localPosition;
        SwayAndBob.AimRotationOffset = Quaternion.Euler(transform.localEulerAngles);
    }


    void Aim()
    {
        // if (_isAim == true)
        // {
        //     _elapsedTime += Time.deltaTime;
        //     float t = Mathf.Clamp01(_elapsedTime / _moveDuration);

        //     transform.localPosition = Vector3.Lerp(_normalPos, _aimPos, t);
        //     transform.localRotation = Quaternion.Slerp(_normalRot, Quaternion.Euler(_aimRot), t);

        //     if (t >= 1f)
        //     {
        //         _isAim = false;
        //         _elapsedTime = 0;
        //     }
        // }

        // else if (_isUnAim == true)
        // {
        //     _elapsedTime += Time.deltaTime;
        //     float t = Mathf.Clamp01(_elapsedTime / _moveDuration);

        //     transform.localPosition = Vector3.Lerp(_aimPos, _normalPos, t);
        //     transform.localRotation = Quaternion.Slerp(Quaternion.Euler(_aimRot), _normalRot, t);

        //     if (t >= 1f)
        //     {
        //         _isUnAim = false;
        //         _elapsedTime = 0;
        //     }
        // }
    }

    public void PlayGunAudio(Vector3 position)
    {
        gunSound.transform.position = position;
        gunSound.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayGunAudio_ServerRpc(Vector3 position)
    {
        PlayGunAudio(position);
        PlayGunAudio_ClientRpc(position);
    }

    [ClientRpc]
    public void PlayGunAudio_ClientRpc(Vector3 position)
    {
        PlayGunAudio(position);
    }

    public void StopGunAudio(Vector3 position)
    {
        gunSound.transform.position = position;
        gunSound.Stop();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopGunAudio_ServerRpc(Vector3 position)
    {
        StopGunAudio(position);
        StopGunAudio_ClientRpc(position);
    }

    [ClientRpc]
    public void StopGunAudio_ClientRpc(Vector3 position)
    {
        StopGunAudio(position);
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
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        Shoot();
        Aim();
        //OnReload();
    }
}