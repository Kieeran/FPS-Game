using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour, IInitAwake, IInitNetwork
{
    public PlayerRoot PlayerRoot { get; private set; }
    CinemachineVirtualCamera _playerCamera;
    public float normalFOV;
    float _currentAimFOV;
    public float fovSpeed;

    bool _isAim;
    bool _isUnAim;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    // OnNetworkSpawn
    public int PriorityNetwork => 15;
    public void InitializeOnNetworkSpawn()
    {
        _playerCamera = InGameManager.Instance.GetCinemachineVirtualCamera();

        _isAim = false;
        _isUnAim = false;

        PlayerRoot.PlayerAim.OnAim += () =>
        {
            _isAim = true;
            _isUnAim = false;
        };

        PlayerRoot.PlayerAim.OnUnAim += () =>
        {
            _isAim = false;
            _isUnAim = true;
        };

        PlayerRoot.WeaponHolder.OnChangeWeapon += OnChangeWeapon;
    }

    public void SetFOV(float fov)
    {
        _playerCamera.m_Lens.FieldOfView = fov;
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
        if (!IsOwner) return;

        if (_isAim == true)
        {
            _playerCamera.m_Lens.FieldOfView = Mathf.Lerp(
                _playerCamera.m_Lens.FieldOfView,
                _currentAimFOV,
                Time.deltaTime * fovSpeed
            );

            if (_playerCamera.m_Lens.FieldOfView < 30f && PlayerRoot.PlayerUI.CurrentPlayerCanvas.ScopeAim.gameObject.activeSelf == false)
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

            if (PlayerRoot.PlayerUI?.CurrentPlayerCanvas?.ScopeAim.gameObject.activeSelf == true)
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
        PlayerRoot.PlayerUI.CurrentPlayerCanvas.ScopeAim.gameObject.SetActive(b);
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
