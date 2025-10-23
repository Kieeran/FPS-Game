using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : PlayerBehaviour
{
    public bool ToggleAim { get; private set; }
    
    // Awake
    public override void InitializeAwake()
    {
        base.InitializeAwake();
        ToggleAim = false;
    }

    // OnNetworkSpawn
    public override int PriorityNetwork => 15;
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        PlayerRoot.Events.OnWeaponChanged += OnWeaponChanged;
    }

    private void OnWeaponChanged(object sender, PlayerEvents.WeaponEventArgs e)
    {
        ToggleAim = false;
    }

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerUI.IsEscapeUIOn()) return;

        if (PlayerRoot.PlayerAssetsInputs.aim == true)
        {
            PlayerRoot.PlayerAssetsInputs.aim = false;
            ToggleAim = !ToggleAim;

            PlayerRoot.Events.InvokeAimStateChanged(ToggleAim);
        }
    }
}
