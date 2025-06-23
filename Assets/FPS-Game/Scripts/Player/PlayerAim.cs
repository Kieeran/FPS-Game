using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour, IInitAwake, IInitNetwork
{
    public PlayerRoot PlayerRoot { get; private set; }
    public Action OnAim;
    public Action OnUnAim;
    public bool ToggleAim { get; private set; }

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
        ToggleAim = false;
    }

    // OnNetworkSpawn
    public int PriorityNetwork => 15;
    public void InitializeOnNetworkSpawn()
    {
        PlayerRoot.WeaponHolder.OnChangeWeapon += OnChangeWeapon;
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
            }

            else
            {
                OnUnAim?.Invoke();
            }
        }
    }
}
