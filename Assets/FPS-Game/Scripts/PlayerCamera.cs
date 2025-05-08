using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] PlayerAim _playerAim;
    [SerializeField] WeaponHolder _weaponHolder;
    [SerializeField] Image _scopeAim;
    CinemachineVirtualCamera _playerCamera;
    public float normalFOV;
    float _currentAimFOV;
    public float fovSpeed;

    bool _isAim;
    bool _isUnAim;

    public void SetFOV(float fov)
    {
        _playerCamera.m_Lens.FieldOfView = fov;
    }

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

        _weaponHolder.OnChangeWeapon += OnChangeWeapon;
    }

    void OnChangeWeapon(object sender, WeaponHolder.WeaponEventArgs e)
    {
        if (e.CurrentWeapon.TryGetComponent<Gun>(out var currentGun))
        {
            _currentAimFOV = currentGun.GetAimFOV();
        }

        else
        {
            _currentAimFOV = normalFOV;
        }

        _isAim = false;
        _isUnAim = true;
    }

    void Update()
    {
        if (_isAim == true)
        {
            _playerCamera.m_Lens.FieldOfView = Mathf.Lerp(
                _playerCamera.m_Lens.FieldOfView,
                _currentAimFOV,
                Time.deltaTime * fovSpeed
            );

            if (_playerCamera.m_Lens.FieldOfView < 30f && _scopeAim.gameObject.activeSelf == false)
            {
                UpdateScopeAimUI(true);
            }

            if (Mathf.Abs(_playerCamera.m_Lens.FieldOfView - _currentAimFOV) <= 0.01f)
            {
                _playerCamera.m_Lens.FieldOfView = _currentAimFOV;
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

            if (_scopeAim.gameObject.activeSelf == true)
            {
                UpdateScopeAimUI(false);
            }

            if (Mathf.Abs(_playerCamera.m_Lens.FieldOfView - normalFOV) <= 0.01f)
            {
                _playerCamera.m_Lens.FieldOfView = normalFOV;
                _isUnAim = false;
            }
        }
    }

    public void UpdateScopeAimUI(bool b)
    {
        _scopeAim.gameObject.SetActive(b);
    }

    public void UnAimScope()
    {
        UpdateScopeAimUI(false);
        _playerCamera.m_Lens.FieldOfView = normalFOV;
        _isAim = false;
        _isUnAim = true;
    }

    public void AimScope()
    {
        _isAim = true;
        _isUnAim = false;
    }
}
