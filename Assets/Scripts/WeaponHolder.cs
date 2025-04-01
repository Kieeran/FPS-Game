using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class WeaponHolder : NetworkBehaviour
{
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;

    [SerializeField] private GameObject _primaryWeapon;
    [SerializeField] private GameObject _secondaryWeapon;
    [SerializeField] private GameObject _grenades;

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

        if (playerAssetsInputs.hotkey1)
        {
            playerAssetsInputs.hotkey1 = false;
            RequestEquipWeapon_ServerRpc(1);
        }
        else if (playerAssetsInputs.hotkey2)
        {
            playerAssetsInputs.hotkey2 = false;
            RequestEquipWeapon_ServerRpc(2);
        }
        else if (playerAssetsInputs.hotkey3)
        {
            playerAssetsInputs.hotkey3 = false;
            RequestEquipWeapon_ServerRpc(3);
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
        _grenades.SetActive(weaponIndex == 3);
    }
}
