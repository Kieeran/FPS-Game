using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class WeaponHolder : NetworkBehaviour, IInitAwake, IInitNetwork
{
    public PlayerRoot PlayerRoot { get; private set; }
    public WeaponMountPoint WeaponMountPoint;
    public Gun Rifle;
    public Gun Sniper;
    public Gun Pistol;

    public Action<GunType> OnChangeGun;

    List<GameObject> _weaponList;

    int _currentWeaponIndex;

    public event EventHandler<WeaponEventArgs> OnChangeWeapon;
    public class WeaponEventArgs : EventArgs
    {
        public GameObject CurrentWeapon;
    }

    public List<GameObject> GetWeaponList() { return _weaponList; }

    public Rigidbody Rb { get; private set; }

    Vector3 originWeaponHolderPos;
    Quaternion originWeaponHolderRot;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();

        _weaponList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == true)
                _weaponList.Add(child.gameObject);
        }

        SetOrigin();

        Rb = gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<NetworkRigidbody>();
        StartCoroutine(SetKinematicNextFrame());
    }

    // OnNetworkSpawn
    public int PriorityNetwork => 20;
    public void InitializeOnNetworkSpawn()
    {
        StartCoroutine(SetFirstWeapon());
    }

    IEnumerator SetFirstWeapon()
    {
        yield return null;

        _currentWeaponIndex = 0;
        OnChangeWeapon?.Invoke(this, new WeaponEventArgs { CurrentWeapon = GetCurrentWeapon() });
        EquipWeapon(_currentWeaponIndex);
    }

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        if (PlayerRoot.PlayerAssetsInputs.hotkey1)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey1 = false;
            if (_currentWeaponIndex == 0) return;
            OnChangeGun?.Invoke(GunType.Rifle);
            _currentWeaponIndex = 0;
            ChangeWeapon();
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey2)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey2 = false;
            if (_currentWeaponIndex == 1) return;
            OnChangeGun?.Invoke(GunType.Sniper);
            _currentWeaponIndex = 1;
            ChangeWeapon();
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey3)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey3 = false;
            if (_currentWeaponIndex == 2) return;
            OnChangeGun?.Invoke(GunType.Pistol);
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

    void LateUpdate()
    {
        // Cập nhật vị trí và hướng theo weaponMountPoint
        transform.SetPositionAndRotation(WeaponMountPoint.transform.position, WeaponMountPoint.transform.rotation);
    }

    void ChangeWeapon()
    {
        OnChangeWeapon.Invoke(this, new WeaponEventArgs { CurrentWeapon = GetCurrentWeapon() });

        RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
        PlayerRoot.PlayerUI.CurrentPlayerCanvas.WeaponHud.EquipWeaponUI(_currentWeaponIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestEquipWeapon_ServerRpc(int weaponIndex)
    {
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

    IEnumerator SetKinematicNextFrame()
    {
        yield return null; // đợi 1 frame

        ResetWeaponHolder();
    }

    void SetOrigin()
    {
        originWeaponHolderPos = transform.localPosition;
        originWeaponHolderRot = transform.localRotation;
    }

    public void DropWeapon()
    {
        Rb.isKinematic = false;
    }

    public void ResetWeaponHolder()
    {
        Rb.isKinematic = true;
        transform.SetLocalPositionAndRotation(originWeaponHolderPos, originWeaponHolderRot);
    }

    public GameObject GetCurrentWeapon()
    {
        return _weaponList[_currentWeaponIndex];
    }
}