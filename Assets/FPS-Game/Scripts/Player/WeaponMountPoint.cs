using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponMountPoint : NetworkBehaviour
{
    public WeaponHolder WeaponHolder;
    [SerializeField] List<WeaponPoseSO> _weaponPoseNetworkSO;

    GunType _currentGuntype;

    public override void OnNetworkSpawn()
    {
        _currentGuntype = GunType.Rifle;
        WeaponHolder.OnChangeGun += (GunType) =>
        {
            _currentGuntype = GunType;
            ApplyPose(_currentGuntype, PlayerWeaponPose.Idle);
        };

        ApplyPose(_currentGuntype, PlayerWeaponPose.Idle);
    }

    public void ApplyPose(GunType gunType, PlayerWeaponPose pose)
    {
        foreach (var poseSO in _weaponPoseNetworkSO)
        {
            if (poseSO.GunType == gunType)
            {
                if (poseSO.TryGetPose(pose, out var data))
                {
                    transform.SetLocalPositionAndRotation(data.Position, Quaternion.Euler(data.EulerRotation));
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
