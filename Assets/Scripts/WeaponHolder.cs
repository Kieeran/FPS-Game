using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class WeaponHolder : NetworkBehaviour
{
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;

    [SerializeField] private GameObject _primaryWeapon;
    [SerializeField] private GameObject _secondaryWeapon;
    [SerializeField] private GameObject _grenades;

    private void Start()
    {
        if (!IsOwner) return;

        RequestEquipWeaponServerRpc(1);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (playerAssetsInputs.hotkey1)
        {
            playerAssetsInputs.hotkey1 = false;
            RequestEquipWeaponServerRpc(1);
        }
        else if (playerAssetsInputs.hotkey2)
        {
            playerAssetsInputs.hotkey2 = false;
            RequestEquipWeaponServerRpc(2);
        }
        else if (playerAssetsInputs.hotkey3)
        {
            playerAssetsInputs.hotkey3 = false;
            RequestEquipWeaponServerRpc(3);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestEquipWeaponServerRpc(int weaponIndex)
    {
        EquipWeapon(weaponIndex);
        UpdateWeaponClientRpc(weaponIndex);
    }

    [ClientRpc]
    private void UpdateWeaponClientRpc(int weaponIndex)
    {
        EquipWeapon(weaponIndex);
    }

    private void EquipWeapon(int weaponIndex)
    {
        _primaryWeapon.SetActive(weaponIndex == 1);
        _secondaryWeapon.SetActive(weaponIndex == 2);
        _grenades.SetActive(weaponIndex == 3);
    }
}
