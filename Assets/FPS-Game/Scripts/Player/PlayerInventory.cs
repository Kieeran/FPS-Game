using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour, IInitAwake, IInitStart, IInitNetwork
{
    public PlayerRoot PlayerRoot { get; private set; }

    GameObject _currentWeapon;
    SupplyLoad _currentWeaponSupplyLoad;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    // Start
    public int PriorityStart => 1000;
    public void InitializeStart()
    {
        _currentWeapon = null;
    }

    // OnNetworkSpawn
    public int PriorityNetwork => 15;
    public void InitializeOnNetworkSpawn()
    {
        PlayerRoot.WeaponHolder.OnChangeWeapon += SetCurrentWeapon;
        PlayerRoot.PlayerReload.OnReload += Reload;
        PlayerRoot.PlayerCollision.OnCollectedHealthPickup += RefillAmmos;
    }

    void OnDisable()
    {
        PlayerRoot.WeaponHolder.OnChangeWeapon -= SetCurrentWeapon;
        PlayerRoot.PlayerReload.OnReload -= Reload;
    }

    public void RefillAmmos()
    {
        foreach (GameObject weapon in PlayerRoot.WeaponHolder.GetWeaponList())
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
            PlayerRoot.PlayerReload.ResetIsReloading();
            return;
        }

        int ammoToReload = _currentWeaponSupplyLoad.Capacity - _currentWeaponSupplyLoad.CurrentMagazineAmmo;

        if (ammoToReload == 0)
        {
            PlayerRoot.PlayerReload.ResetIsReloading();
            return;
        }

        else if (ammoToReload > _currentWeaponSupplyLoad.TotalSupplies)
        {
            _currentWeaponSupplyLoad.CurrentMagazineAmmo += _currentWeaponSupplyLoad.TotalSupplies;
            _currentWeaponSupplyLoad.TotalSupplies = 0;

            PlayerRoot.PlayerUI.CurrentPlayerCanvas.BulletHud.ReloadEffect.StartReloadEffect(() =>
            {
                SetAmmoInfoUI();
                PlayerRoot.PlayerReload.ResetIsReloading();
            });
        }

        else
        {
            _currentWeaponSupplyLoad.CurrentMagazineAmmo += ammoToReload;
            _currentWeaponSupplyLoad.TotalSupplies -= ammoToReload;

            PlayerRoot.PlayerUI.CurrentPlayerCanvas.BulletHud.ReloadEffect.StartReloadEffect(() =>
            {
                SetAmmoInfoUI();
                PlayerRoot.PlayerReload.ResetIsReloading();
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

            if (IsOwner)
                SetAmmoInfoUI();
            return;
        }
        _currentWeaponSupplyLoad = null;
        PlayerRoot.PlayerUI.CurrentPlayerCanvas.BulletHud.SetAmmoInfoUI(0, 0);
    }

    public void UpdatecurrentMagazineAmmo()
    {
        _currentWeaponSupplyLoad.CurrentMagazineAmmo--;

        SetAmmoInfoUI();
    }

    void SetAmmoInfoUI()
    {
        PlayerRoot.PlayerUI.CurrentPlayerCanvas.BulletHud.SetAmmoInfoUI(
            _currentWeaponSupplyLoad.CurrentMagazineAmmo,
            _currentWeaponSupplyLoad.TotalSupplies
        );
    }
}
