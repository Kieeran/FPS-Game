using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerReload : PlayerBehaviour
{
    [Header("Weapon")]
    [SerializeField] GameObject _rifle;
    [SerializeField] GameObject _sniper;
    [SerializeField] GameObject _pistol;
    [SerializeField] GameObject _knife;

    [Header("Weapon sound effect")]
    [SerializeField] GameObject _rifleReloadAudio;
    [SerializeField] GameObject _sniperReloadAudio;
    [SerializeField] GameObject _pistolReloadAudio;

    public bool IsReloading { get; private set; }
    public void ResetIsReloading() { IsReloading = false; }

    float reloadCoolDown;

    void Start()
    {
        PlayerRoot.Events.OnWeaponAmmoDepleted += Reload;
        PlayerRoot.Events.OnDoneReload += ResetIsReloading;
        PlayerRoot.Events.OnWeaponChanged += OnWeaponChanged;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.reload == true)
        {
            PlayerRoot.PlayerAssetsInputs.reload = false;
            Reload();
        }

        Tick(Time.deltaTime);
    }

    void Reload()
    {
        if (_knife.activeSelf) return;
        if (IsReloading == true) return;

        IsReloading = true;
        PlayerRoot.Events.InvokeOnReload();

        if (_rifle.activeSelf) StartCoroutine(PlayRifleReloadAudio());
        if (_sniper.activeSelf) StartCoroutine(PlaySniperReloadAudio());
        if (_pistol.activeSelf) StartCoroutine(PlayPistolReloadAudio());

        StartTimer(reloadCoolDown, () =>
        {
            PlayerRoot.Events.InvokeOnDoneReload();
            ResetIsReloading();
        });
    }

    IEnumerator PlayRifleReloadAudio()
    {
        _rifleReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        _rifleReloadAudio.SetActive(false);
    }

    IEnumerator PlaySniperReloadAudio()
    {
        _sniperReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        _sniperReloadAudio.SetActive(false);
    }

    IEnumerator PlayPistolReloadAudio()
    {
        _pistolReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        _pistolReloadAudio.SetActive(false);
    }

    void OnWeaponChanged(object sender, PlayerEvents.WeaponEventArgs e)
    {
        if (e.CurrentWeapon.TryGetComponent<Gun>(out var gun))
        {
            reloadCoolDown = gun.ReloadCoolDown;
        }

        // Khi đang reload giữa chừng mà đổi vũ khí thì không reload nữa
        ResetIsReloading();
    }

    #region Timer
    bool IsRunning = false;
    float RemainingTime;
    Action onFinishedTimer;

    void StartTimer(float duration, Action onFinished = null)
    {
        if (IsRunning == true) return;

        RemainingTime = duration;
        IsRunning = true;
        onFinishedTimer = onFinished;
    }

    void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        RemainingTime -= deltaTime;

        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            IsRunning = false;
            onFinishedTimer?.Invoke();
        }
    }
    #endregion
}