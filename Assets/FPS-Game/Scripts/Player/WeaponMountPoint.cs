using System.Collections.Generic;
using UnityEngine;

public class WeaponMountPoint : MonoBehaviour
{
    public WeaponHolder WeaponHolder;
    public List<WeaponPoseSO> weaponPoseSOs;

    GunType _currentGuntype;

    void Awake()
    {
        _currentGuntype = GunType.Rifle;
        WeaponHolder.OnChangeGun += (GunType) =>
        {
            _currentGuntype = GunType;
            ApplyPose(_currentGuntype, PlayerWeaponPose.Idle);
        };

        WeaponHolder.PlayerRoot.PlayerAim.OnAim += () =>
        {
            ApplyPose(_currentGuntype, PlayerWeaponPose.Aim);
        };

        WeaponHolder.PlayerRoot.PlayerAim.OnUnAim += () =>
        {
            ApplyPose(_currentGuntype, PlayerWeaponPose.Idle);
        };
    }

    void Start()
    {
        ApplyPose(_currentGuntype, PlayerWeaponPose.Idle);
    }

    public void ApplyPose(GunType gunType, PlayerWeaponPose pose)
    {
        foreach (var poseSO in weaponPoseSOs)
        {
            if (poseSO.GunType == gunType)
            {
                if (poseSO.TryGetPose(pose, out var data))
                {
                    // transform.SetLocalPositionAndRotation(data.Position, Quaternion.Euler(data.EulerRotation));
                    transform.localPosition = data.Position;
                    return;
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy pose {pose} trong SO của {gunType}");
                    return;
                }
            }
        }
        Debug.LogWarning($"Không tìm thấy WeaponPoseSO cho loại súng: {gunType}");
    }
}
