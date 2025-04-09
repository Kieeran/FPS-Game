using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] PlayerUI _playerUI;
    [SerializeField] PlayerReload _playerReload;
    [SerializeField] WeaponHolder _weaponHolder;

    GameObject _currentWeapon;
    SupplyLoad _currentWeaponSupplyLoad;

    void Start()
    {
        _currentWeapon = null;

        _weaponHolder.OnChangeWeapon += SetCurrentWeapon;
        _playerReload.OnReload += Reload;
    }

    void OnDestroy()
    {
        _weaponHolder.OnChangeWeapon -= SetCurrentWeapon;
        _playerReload.OnReload -= Reload;
    }

    public void RefillAmmos()
    {
        foreach (GameObject weapon in _weaponHolder.GetWeaponList())
        {
            if (weapon.TryGetComponent<SupplyLoad>(out var supplyLoad))
            {
                supplyLoad.RefillAmmo();
            }
        }

        SetAmmoInfoUI();
    }

    void Reload(object sender, System.EventArgs e)
    {
        if (_currentWeaponSupplyLoad == null || _currentWeaponSupplyLoad.IsTotalSuppliesEmpty())
        {
            _playerReload.ResetIsReloading();
            return;
        }

        int ammoToReload = _currentWeaponSupplyLoad.Capacity - _currentWeaponSupplyLoad.CurrentMagazineAmmo;

        if (ammoToReload == 0)
        {
            _playerReload.ResetIsReloading();
            return;
        }

        else if (ammoToReload > _currentWeaponSupplyLoad.TotalSupplies)
        {
            _currentWeaponSupplyLoad.CurrentMagazineAmmo += _currentWeaponSupplyLoad.TotalSupplies;
            _currentWeaponSupplyLoad.TotalSupplies = 0;

            _playerUI.StartReloadEffect(() =>
            {
                SetAmmoInfoUI();
                _playerReload.ResetIsReloading();
            });
        }

        else
        {
            _currentWeaponSupplyLoad.CurrentMagazineAmmo += ammoToReload;
            _currentWeaponSupplyLoad.TotalSupplies -= ammoToReload;

            _playerUI.StartReloadEffect(() =>
            {
                SetAmmoInfoUI();
                _playerReload.ResetIsReloading();
            });
        }
    }

    void SetCurrentWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        _currentWeapon = e.CurrentWeapon;

        if (_currentWeapon.TryGetComponent<SupplyLoad>(out var supplyLoad))
        {
            _currentWeaponSupplyLoad = supplyLoad;

            _currentWeaponSupplyLoad.EnsureInitialized();

            SetAmmoInfoUI();
            return;
        }
        _currentWeaponSupplyLoad = null;
        _playerUI.SetAmmoInfoUI(0, 0);
    }

    public void UpdatecurrentMagazineAmmo()
    {
        _currentWeaponSupplyLoad.CurrentMagazineAmmo--;

        SetAmmoInfoUI();
    }

    void SetAmmoInfoUI()
    {
        _playerUI.SetAmmoInfoUI(
            _currentWeaponSupplyLoad.CurrentMagazineAmmo,
            _currentWeaponSupplyLoad.TotalSupplies
        );
    }
}
