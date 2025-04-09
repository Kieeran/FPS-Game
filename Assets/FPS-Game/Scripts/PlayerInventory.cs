using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] PlayerUI _playerUI;
    [SerializeField] WeaponHolder _weaponHolder;

    GameObject _currentWeapon;
    SupplyLoad _currentWeaponSupplyLoad;

    bool _isReloading;
    void Start()
    {
        _currentWeapon = null;

        _weaponHolder.OnChangeWeapon += SetCurrentWeapon;
    }

    void OnDestroy()
    {
        _weaponHolder.OnChangeWeapon -= SetCurrentWeapon;
    }

    void SetCurrentWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        _currentWeapon = e.CurrentWeapon;

        if (_currentWeapon.TryGetComponent<SupplyLoad>(out var supplyLoad))
        {
            _currentWeaponSupplyLoad = supplyLoad;

            _playerUI.SetAmmoInfo(
                _currentWeaponSupplyLoad.CurrentMagazineAmmo,
                _currentWeaponSupplyLoad.TotalSupplies
            );
            return;
        }
        _currentWeaponSupplyLoad = null;
        _playerUI.SetAmmoInfo(0, 0);
    }

    public void UpdatecurrentMagazineAmmo()
    {
        _currentWeaponSupplyLoad.CurrentMagazineAmmo--;

        _playerUI.SetAmmoInfo(
            _currentWeaponSupplyLoad.CurrentMagazineAmmo,
            _currentWeaponSupplyLoad.TotalSupplies
        );
    }
}
