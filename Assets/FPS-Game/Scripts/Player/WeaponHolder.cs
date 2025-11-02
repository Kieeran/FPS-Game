using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class WeaponHolder : PlayerBehaviour
{
    [Header("Weapon Pose SO")]
    [SerializeField] List<WeaponPoseSO> _weaponPoseLocalSO;
    public Dictionary<GunType, WeaponPoseSO> WeaponPoseLocalSOs;
    [Space(10)]
    public Transform WeaponMountPoint;
    public Gun Rifle;
    public Gun Sniper;
    public Gun Pistol;

    List<GameObject> _weaponList;

    int _currentWeaponIndex;

    public List<GameObject> GetWeaponList() { return _weaponList; }

    public Rigidbody Rb { get; private set; }

    Vector3 originWeaponHolderPos;
    Quaternion originWeaponHolderRot;

    public override void InitializeAwake()
    {
        base.InitializeAwake();
        _weaponList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == true)
                _weaponList.Add(child.gameObject);
        }

        SetOrigin();
        InitializeDictionary();

        if (gameObject.TryGetComponent<Rigidbody>(out var rb)) Rb = rb;
        else Rb = gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<NetworkRigidbody>();
        StartCoroutine(SetKinematicNextFrame());
    }

    // OnNetworkSpawn
    public override int PriorityNetwork => 20;
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        if (IsOwner & IsLocalPlayer && !PlayerRoot.IsCharacterBot())
        {
            Vector3 localScale = new(1.6f, 1.6f, 1.6f);

            Rifle.transform.localScale = localScale;
            Sniper.transform.localScale = localScale;
            Pistol.transform.localScale = localScale;
        }

        StartCoroutine(SetFirstWeapon());
    }

    void InitializeDictionary()
    {
        WeaponPoseLocalSOs = new();

        foreach (var so in _weaponPoseLocalSO)
        {
            WeaponPoseLocalSOs.Add(so.GunType, so);
        }
    }

    IEnumerator SetFirstWeapon()
    {
        yield return null;

        _currentWeaponIndex = 0;
        PlayerRoot.Events.InvokeWeaponChanged(GetCurrentWeapon(), GunType.Rifle);
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
            _currentWeaponIndex = 0;
            ChangeWeapon(GunType.Rifle);
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey2)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey2 = false;
            if (_currentWeaponIndex == 1) return;
            _currentWeaponIndex = 1;
            ChangeWeapon(GunType.Sniper);
        }

        else if (PlayerRoot.PlayerAssetsInputs.hotkey3)
        {
            PlayerRoot.PlayerAssetsInputs.hotkey3 = false;
            if (_currentWeaponIndex == 2) return;
            _currentWeaponIndex = 2;
            ChangeWeapon(GunType.Pistol);
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
        if (IsOwner & IsLocalPlayer) return;

        // Cập nhật vị trí và hướng theo weaponMountPoint
        transform.SetPositionAndRotation(WeaponMountPoint.position, WeaponMountPoint.rotation);
    }

    void ChangeWeapon(GunType gunType = GunType.None)
    {
        PlayerRoot.Events.InvokeWeaponChanged(GetCurrentWeapon(), gunType);
        RequestEquipWeapon_ServerRpc(_currentWeaponIndex);
        if (!PlayerRoot.IsCharacterBot() && PlayerRoot.PlayerUI != null)
        {
            PlayerRoot.PlayerUI.CurrentPlayerCanvas.WeaponHud.EquipWeaponUI(_currentWeaponIndex);
        }
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