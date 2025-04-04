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

    void Start()
    {
        EquipWeapon(1);
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
            RequestEquipWeapon_ServerRpc(1);

            _playerUI.GetWeaponHud().EquipWeaponUI(1);
        }

        else if (_playerAssetsInputs.hotkey2)
        {
            _playerAssetsInputs.hotkey2 = false;
            RequestEquipWeapon_ServerRpc(2);

            _playerUI.GetWeaponHud().EquipWeaponUI(2);
        }

        else if (_playerAssetsInputs.hotkey3)
        {
            _playerAssetsInputs.hotkey3 = false;
            RequestEquipWeapon_ServerRpc(3);

            _playerUI.GetWeaponHud().EquipWeaponUI(3);
        }

        else if (_playerAssetsInputs.hotkey4)
        {
            _playerAssetsInputs.hotkey4 = false;
            RequestEquipWeapon_ServerRpc(4);

            _playerUI.GetWeaponHud().EquipWeaponUI(4);
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
        _primaryWeapon.SetActive(weaponIndex == 1);
        _secondaryWeapon.SetActive(weaponIndex == 2);
        _meleeWeapon.SetActive(weaponIndex == 3);
        _grenades.SetActive(weaponIndex == 4);
    }
}
