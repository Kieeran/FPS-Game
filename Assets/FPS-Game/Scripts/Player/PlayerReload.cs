using System;
using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }

    bool _isReloading;
    public bool GetIsReloading() { return _isReloading; }
    public void ResetIsReloading() { _isReloading = false; }

    public event EventHandler OnReload;
    public Action reload;

    void Awake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    void Update()
    {
        if (PlayerRoot.PlayerAssetsInputs.reload == true)
        {
            reload?.Invoke();
            PlayerRoot.PlayerAssetsInputs.reload = false;

            if (_isReloading != true)
            {
                _isReloading = true;
                OnReload.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
