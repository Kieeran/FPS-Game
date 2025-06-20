using System.Collections.Generic;
using UnityEngine;

public class WeaponMountPoint : MonoBehaviour
{
    public WeaponHolder WeaponHolder;
    public List<WeaponPoseSO> weaponPoseSOs;

    void Awake()
    {
        WeaponHolder.OnChangeGun += (GunType) =>
        {
            ApplyPose(GunType, PlayerWeaponPose.Idle);
        };
    }

    void Start()
    {
        ApplyPose(GunType.Rifle, PlayerWeaponPose.Idle);
    }

    public void ApplyPose(GunType gunType, PlayerWeaponPose pose)
    {
        foreach (var poseSO in weaponPoseSOs)
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
