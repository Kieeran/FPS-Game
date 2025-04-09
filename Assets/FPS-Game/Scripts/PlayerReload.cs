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

    void Start()
    {

    }

    void Update()
    {
        if (_playerAssetsInputs.reload == true)
        {
            _playerAssetsInputs.reload = false;

            if (_isReloading != true)
            {
                _isReloading = true;
                OnReload.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
