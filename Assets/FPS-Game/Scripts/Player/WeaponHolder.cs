using System;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class WeaponHolder : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }

    List<GameObject> _weaponList;

    int _currentWeaponIndex;

    public event EventHandler<WeaponEventArgs> OnChangeWeapon;
    public class WeaponEventArgs : EventArgs
    {
        public GameObject CurrentWeapon;
    }

    public List<GameObject> GetWeaponList() { return _weaponList; }

    void Start()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();

        _weaponList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == true)
                _weaponList.Add(child.gameObject);
        }

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
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        if (PlayerRoot.PlayerAssetsInputs.hotkey1)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey1 = false;
            if (_currentWeaponIndex == 0) return;

            _currentWeaponIndex = 0;
            ChangeWeapon();
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey2)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey2 = false;
            if (_currentWeaponIndex == 1) return;

            _currentWeaponIndex = 1;
            ChangeWeapon();
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey3)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey3 = false;
            if (_currentWeaponIndex == 2) return;

            _currentWeaponIndex = 2;
            ChangeWeapon();
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey4)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey4 = false;
            if (_currentWeaponIndex == 3) return;

            _currentWeaponIndex = 3;
            ChangeWeapon();
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey5)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey5 = false;
            if (_currentWeaponIndex == 4) return;

            _currentWeaponIndex = 4;
            ChangeWeapon();
        }
    }

    void ChangeWeapon()
    {
        OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = _weaponList[_currentWeaponIndex] });

        RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
        PlayerRoot.PlayerUI.GetWeaponHud().EquipWeaponUI(_currentWeaponIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestEquipWeapon_ServerRpc(int weaponIndex)
    {
        EquipWeapon(weaponIndex);
        UpdateWeapon_ClientRpc(weaponIndex);
    }

    [ClientRpc]
    void UpdateWeapon_ClientRpc(int weaponIndex)
    {
        EquipWeapon(weaponIndex);
    }

    void EquipWeapon(int weaponIndex)
    {
        for (int i = 0; i < _weaponList.Count; i++)
        {
            _weaponList[i].SetActive(weaponIndex == i);
        }
    }
}
