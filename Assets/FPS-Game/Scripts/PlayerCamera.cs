using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] PlayerAim _playerAim;
    CinemachineVirtualCamera _playerCamera;
    public float normalFOV;
    public float aimFOV;
    public float fovSpeed;

    bool _isAim;
    bool _isUnAim;

    public override void OnNetworkSpawn()
    {
        _playerCamera = GameManager.Instance.GetCinemachineVirtualCamera();

        _isAim = false;
        _isUnAim = false;

        _playerAim.OnAim += () =>
        {
            _isAim = true;
            _isUnAim = false;
        };

        _playerAim.OnUnAim += () =>
        {
            _isAim = false;
            _isUnAim = true;
        };
    }

    void Update()
    {
        if (_isAim == true)
        {
            _playerCamera.m_Lens.FieldOfView = Mathf.Lerp(
                _playerCamera.m_Lens.FieldOfView,
                aimFOV,
                Time.deltaTime * fovSpeed
            );

            if (Mathf.Abs(_playerCamera.m_Lens.FieldOfView - aimFOV) <= 0.01f)
            {
                _playerCamera.m_Lens.FieldOfView = aimFOV;
                _isAim = false;
            }
        }

        else if (_isUnAim == true)
        {
            _playerCamera.m_Lens.FieldOfView = Mathf.Lerp(
                _playerCamera.m_Lens.FieldOfView,
                normalFOV,
                Time.deltaTime * fovSpeed
            );

            if (Mathf.Abs(_playerCamera.m_Lens.FieldOfView - normalFOV) <= 0.01f)
            {
                _playerCamera.m_Lens.FieldOfView = normalFOV;
                _isUnAim = false;
            }
        }
    }
}
