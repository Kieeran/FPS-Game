using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerReload : NetworkBehaviour, IInitAwake
{
    public PlayerRoot PlayerRoot { get; private set; }

    [Header("Weapon")]
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject sniper;
    [SerializeField] GameObject pistol;

    [Header("Weapon sound effect")]
    [SerializeField] GameObject rifleReloadAudio;
    [SerializeField] GameObject sniperReloadAudio;
    [SerializeField] GameObject pistolReloadAudio;

    bool _isReloading;
    public bool GetIsReloading() { return _isReloading; }
    public void ResetIsReloading() { _isReloading = false; }

    public event EventHandler OnReload;
    public Action reload;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.reload == true)
        {
            if (rifle.activeSelf == true) StartCoroutine(PlayRifleReloadAudio());
            if (sniper.activeSelf == true) StartCoroutine(PlaySniperReloadAudio());
            if (pistol.activeSelf == true) StartCoroutine(PlayPistolReloadAudio());

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
        rifleReloadAudio.SetActive(true);
        yield return new WaitForSeconds(2f);
        rifleReloadAudio.SetActive(false);
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