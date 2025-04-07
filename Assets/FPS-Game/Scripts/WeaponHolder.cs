using System;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class WeaponHolder : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] PlayerUI _playerUI;

    [SerializeField] GameObject _primaryWeapon;
    [SerializeField] GameObject _secondaryWeapon;
    [SerializeField] GameObject _meleeWeapon;
    [SerializeField] GameObject _grenades;

    List<GameObject> _weaponList;

    int _currentWeaponIndex;

    public event EventHandler<WeaponEventArgs> OnChangeWeapon;
    public class WeaponEventArgs : EventArgs
    {
        public GameObject CurrentWeapon;
    }

    void Start()
    {
        _weaponList = new List<GameObject>
        {
            _primaryWeapon,
            _secondaryWeapon,
            _meleeWeapon,
            _grenades
        };

        _currentWeaponIndex = 0;

        OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = _weaponList[_currentWeaponIndex] });
        EquipWeapon(_currentWeaponIndex);
    }

    public override void OnNetworkSpawn()
    {
        // RequestEquipWeapon_ServerRpc(1);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_playerAssetsInputs.hotkey1)
        {
            _playerAssetsInputs.hotkey1 = false;
            if (_currentWeaponIndex == 0) return;

            _currentWeaponIndex = 0;
            OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = _weaponList[_currentWeaponIndex] });

            RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
            _playerUI.GetWeaponHud().EquipWeaponUI(_currentWeaponIndex);
        }

        else if (_playerAssetsInputs.hotkey2)
        {
            _playerAssetsInputs.hotkey2 = false;
            if (_currentWeaponIndex == 1) return;

            _currentWeaponIndex = 1;
            OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = _weaponList[_currentWeaponIndex] });

            RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
            _playerUI.GetWeaponHud().EquipWeaponUI(_currentWeaponIndex);
        }

        else if (_playerAssetsInputs.hotkey3)
        {
            _playerAssetsInputs.hotkey3 = false;
            if (_currentWeaponIndex == 2) return;

            _currentWeaponIndex = 2;
            OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = _weaponList[_currentWeaponIndex] });

            RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
            _playerUI.GetWeaponHud().EquipWeaponUI(_currentWeaponIndex);
        }

        else if (_playerAssetsInputs.hotkey4)
        {
            _playerAssetsInputs.hotkey4 = false;
            if (_currentWeaponIndex == 3) return;

            _currentWeaponIndex = 3;
            OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = _weaponList[_currentWeaponIndex] });

            RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
            _playerUI.GetWeaponHud().EquipWeaponUI(_currentWeaponIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestEquipWeapon_ServerRpc(int weaponIndex)
    {
        EquipWeapon(weaponIndex);
        UpdateWeapon_ClientRpc(weaponIndex);
    }

    [ClientRpc]
    private void UpdateWeapon_ClientRpc(int weaponIndex)
    {
        EquipWeapon(weaponIndex);
    }

    private void EquipWeapon(int weaponIndex)
    {
        _primaryWeapon.SetActive(weaponIndex == 0);
        _secondaryWeapon.SetActive(weaponIndex == 1);
        _meleeWeapon.SetActive(weaponIndex == 2);
        _grenades.SetActive(weaponIndex == 3);
    }
}
