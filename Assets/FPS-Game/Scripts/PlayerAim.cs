using System;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    public Action OnAim;
    public Action OnUnAim;

    bool _toggleAim;

    void Start()
    {
        _toggleAim = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_playerAssetsInputs.aim == true)
        {
            _playerAssetsInputs.aim = false;

            _toggleAim = !_toggleAim;

            if (_toggleAim == true)
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
