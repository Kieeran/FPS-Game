using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class WeaponSwitching : NetworkBehaviour
{
    public GameObject[] weapons;
    public InputActionAsset inputActionAsset; // Reference to the Input Action Asset

    private NetworkVariable<int> currentWeaponIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private InputAction switchToWeapon1;
    private InputAction switchToWeapon2;
    private InputAction switchToWeapon3;
    private InputAction switchToWeapon4;

    public override void OnNetworkSpawn()
    {
        weapons[1].SetActive(false);
        weapons[2].SetActive(false);
        weapons[3].SetActive(false);
    }

    private void Start()
    {
        if (IsOwner)
        {
            ActivateWeapon(0); // Default active weapon
        }

        currentWeaponIndex.OnValueChanged += (oldIndex, newIndex) => ActivateWeapon(newIndex);
    }

    private void OnEnable()
    {
        if (inputActionAsset == null)
        {
            Debug.LogError("InputActionAsset is not assigned!");
            return;
        }

        switchToWeapon1 = inputActionAsset.FindAction("SwitchToWeapon1");
        switchToWeapon2 = inputActionAsset.FindAction("SwitchToWeapon2");
        switchToWeapon3 = inputActionAsset.FindAction("SwitchToWeapon3");
        switchToWeapon4 = inputActionAsset.FindAction("SwitchToWeapon4");

        if (switchToWeapon1 != null) switchToWeapon1.performed += _ => OnSwitchWeapon(0);
        if (switchToWeapon2 != null) switchToWeapon2.performed += _ => OnSwitchWeapon(1);
        if (switchToWeapon3 != null) switchToWeapon3.performed += _ => OnSwitchWeapon(2);
        if (switchToWeapon4 != null) switchToWeapon4.performed += _ => OnSwitchWeapon(3);

        switchToWeapon1?.Enable();
        switchToWeapon2?.Enable();
        switchToWeapon3?.Enable();
        switchToWeapon4?.Enable();
    }

    private void OnDisable()
    {
        switchToWeapon1?.Disable();
        switchToWeapon2?.Disable();
        switchToWeapon3?.Disable();
        switchToWeapon4?.Disable();

        if (switchToWeapon1 != null) switchToWeapon1.performed -= _ => OnSwitchWeapon(0);
        if (switchToWeapon2 != null) switchToWeapon2.performed -= _ => OnSwitchWeapon(1);
        if (switchToWeapon3 != null) switchToWeapon3.performed -= _ => OnSwitchWeapon(2);
        if (switchToWeapon4 != null) switchToWeapon4.performed -= _ => OnSwitchWeapon(3);
    }

    private void OnSwitchWeapon(int weaponIndex)
    {
        if (IsOwner && weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            RequestWeaponSwitchServerRpc(weaponIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestWeaponSwitchServerRpc(int weaponIndex, ServerRpcParams rpcParams = default)
    {
        currentWeaponIndex.Value = weaponIndex;
        SyncWeaponStateClientRpc(weaponIndex);
    }

    [ClientRpc]
    private void SyncWeaponStateClientRpc(int weaponIndex)
    {
        ActivateWeapon(weaponIndex);
    }

    private void ActivateWeapon(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == weaponIndex);
        }
    }
}