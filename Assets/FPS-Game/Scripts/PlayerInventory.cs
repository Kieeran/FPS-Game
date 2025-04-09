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

    void Reload(object sender, System.EventArgs e)
    {
        _playerReload.ResetIsReloading();

        if (_currentWeaponSupplyLoad.IsTotalSuppliesEmpty()) return;

        int ammoToReload = _currentWeaponSupplyLoad.Capacity - _currentWeaponSupplyLoad.CurrentMagazineAmmo;

        if (ammoToReload > _currentWeaponSupplyLoad.TotalSupplies)
        {
            _currentWeaponSupplyLoad.CurrentMagazineAmmo += _currentWeaponSupplyLoad.TotalSupplies;
            _currentWeaponSupplyLoad.TotalSupplies = 0;
        }

        else
        {
            _currentWeaponSupplyLoad.CurrentMagazineAmmo += ammoToReload;
            _currentWeaponSupplyLoad.TotalSupplies -= ammoToReload;
        }

        SetAmmoInfoUI();
    }

    void SetCurrentWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        _currentWeapon = e.CurrentWeapon;

        if (_currentWeapon.TryGetComponent<SupplyLoad>(out var supplyLoad))
        {
            _currentWeaponSupplyLoad = supplyLoad;

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
