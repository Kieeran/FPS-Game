using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] WeaponHolder _weaponHolder;

    GameObject _currentWeapon;

    bool _isReloading;
    void Start()
    {
        _currentWeapon = null;

        _weaponHolder.OnChangeWeapon += SetCurrentWeapon;
    }

    void SetCurrentWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        // Debug.Log(e.CurrentWeapon.name);
        _currentWeapon = e.CurrentWeapon;
    }

    void OnDestroy()
    {
        _weaponHolder.OnChangeWeapon -= SetCurrentWeapon;
    }

    void Update()
    {

    }
}
