using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] WeaponHolder _weaponHolder;

    bool _isReloading;
    void Start()
    {
        _weaponHolder.OnChangeWeapon += PrintInfo;
    }

    void PrintInfo(object sender, WeaponHolder.WeaponEventArgs e)
    {
        Debug.Log(e.CurrentWeapon.name);
    }

    void OnDestroy()
    {
        _weaponHolder.OnChangeWeapon -= PrintInfo;
    }

    void Update()
    {

    }
}
