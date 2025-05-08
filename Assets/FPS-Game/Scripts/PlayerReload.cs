using System;
using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class PlayerReload : MonoBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;

    bool _isReloading;
    public bool GetIsReloading() { return _isReloading; }
    public void ResetIsReloading() { _isReloading = false; }

    public event EventHandler OnReload;
    public Action reload;

    void Update()
    {
        if (_playerAssetsInputs.reload == true)
        {
            reload?.Invoke();
            _playerAssetsInputs.reload = false;

            if (_isReloading != true)
            {
                _isReloading = true;
                OnReload.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
