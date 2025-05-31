using System;
using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;

    [SerializeField] GameObject ak47;
    [SerializeField] GameObject ak47ReloadAudio;
    [SerializeField] GameObject sniper;
    [SerializeField] GameObject sniperReloadAudio;
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject pistolReloadAudio;

    bool _isReloading;
    public bool GetIsReloading() { return _isReloading; }
    public void ResetIsReloading() { _isReloading = false; }

    public event EventHandler OnReload;
    public Action reload;

    void Update()
    {
        if (_playerAssetsInputs.reload == true)
        {
            if (ak47.activeSelf == true) StartCoroutine(PlayAk47ReloadAudio());
            if (sniper.activeSelf == true) StartCoroutine(PlaySniperReloadAudio());
            if (pistol.activeSelf == true) StartCoroutine(PlayPistolReloadAudio());

            reload?.Invoke();
            _playerAssetsInputs.reload = false;

            if (_isReloading != true)
            {
                _isReloading = true;
                OnReload.Invoke(this, EventArgs.Empty);
            }

        }
    }

    IEnumerator PlayAk47ReloadAudio()
    {
        ak47ReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        ak47ReloadAudio.SetActive(false);
    }

    IEnumerator PlaySniperReloadAudio()
    {
        sniperReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        sniperReloadAudio.SetActive(false);
    }

    IEnumerator PlayPistolReloadAudio()
    {
        pistolReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        pistolReloadAudio.SetActive(false);
    }
}
