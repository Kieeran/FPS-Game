using System;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    [SerializeField] WeaponHolder _weaponHolder;
    public Action OnAim;
    public Action OnUnAim;

    public bool ToggleAim { get; private set; }

    void Awake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    void Start()
    {
        ToggleAim = false;
    }

    public override void OnNetworkSpawn()
    {
        _weaponHolder.OnChangeWeapon += OnChangeWeapon;
    }

    private void OnChangeWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        ToggleAim = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.aim == true)
        {
            PlayerRoot.PlayerAssetsInputs.aim = false;

            ToggleAim = !ToggleAim;

            if (ToggleAim == true)
            {
                OnAim?.Invoke();
                // Debug.Log("Aim!");
            }

            else
            {
                OnUnAim?.Invoke();
                // Debug.Log("Un Aim!");
            }
        }
    }
}
