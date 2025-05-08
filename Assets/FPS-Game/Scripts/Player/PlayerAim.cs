using System;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] WeaponHolder _weaponHolder;
    public Action OnAim;
    public Action OnUnAim;

    public bool ToggleAim { get; private set; }

    void Start()
    {
        ToggleAim = false;
        _weaponHolder.OnChangeWeapon += OnChangeWeapon;
    }

    private void OnChangeWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        ToggleAim = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_playerAssetsInputs.aim == true)
        {
            _playerAssetsInputs.aim = false;

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
