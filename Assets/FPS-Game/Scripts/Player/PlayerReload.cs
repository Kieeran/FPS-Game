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

    bool _isReloading;
    public bool GetIsReloading() { return _isReloading; }
    public void ResetIsReloading() { _isReloading = false; }

    public event EventHandler OnReload;
    public Action reload;

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.reload == true)
        {
            if (_rifle.activeSelf) StartCoroutine(PlayRifleReloadAudio());
            if (_sniper.activeSelf) StartCoroutine(PlaySniperReloadAudio());
            if (_pistol.activeSelf) StartCoroutine(PlayPistolReloadAudio());

            if (_knife.activeSelf) return;

            reload?.Invoke();
            PlayerRoot.PlayerAssetsInputs.reload = false;

            if (_isReloading != true)
            {
                _isReloading = true;
                OnReload.Invoke(this, EventArgs.Empty);
            }
        }
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
}