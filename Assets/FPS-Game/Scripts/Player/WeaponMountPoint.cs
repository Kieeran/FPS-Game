using System.Collections.Generic;
using UnityEngine;

public enum PlayerWeaponPose
{
    Idle,
    Aim,
}

[System.Serializable]
public struct WeaponTransformData
{
    public GunType GunType;
    public PlayerWeaponPose PoseType;
    public Vector3 Position;
    public Vector3 EulerRotation;

    public WeaponTransformData(GunType gunType, PlayerWeaponPose poseType, Vector3 position, Vector3 eulerRotation)
    {
        GunType = gunType;
        PoseType = poseType;
        Position = position;
        EulerRotation = eulerRotation;
    }
}

public class WeaponMountPoint : MonoBehaviour
{
    public WeaponHolder WeaponHolder;
    // Cặp key là (GunType, PoseType)
    Dictionary<(GunType, PlayerWeaponPose), WeaponTransformData> _poseTransforms = new();

    [Header("Idle Poses")]
    public WeaponTransformData IdlePoseRifle;
    public WeaponTransformData IdlePoseSniper;
    public WeaponTransformData IdlePosePistol;

    [Header("Aim Poses")]
    public WeaponTransformData AimPoseRifle;
    public WeaponTransformData AimPoseSniper;
    public WeaponTransformData AimPosePistol;

    void Awake()
    {
        Initialize();

        WeaponHolder.OnChangeGun += (GunType) =>
        {
            ApplyPose(GunType, PlayerWeaponPose.Idle);
        };
    }

    void Start()
    {
        ApplyPose(GunType.Rifle, PlayerWeaponPose.Idle);
    }

    public void Initialize()
    {
        _poseTransforms[(GunType.Rifle, PlayerWeaponPose.Idle)] = IdlePoseRifle;
        _poseTransforms[(GunType.Sniper, PlayerWeaponPose.Idle)] = IdlePoseSniper;
        _poseTransforms[(GunType.Pistol, PlayerWeaponPose.Idle)] = IdlePosePistol;

        _poseTransforms[(GunType.Rifle, PlayerWeaponPose.Aim)] = AimPoseRifle;
        _poseTransforms[(GunType.Sniper, PlayerWeaponPose.Aim)] = AimPoseSniper;
        _poseTransforms[(GunType.Pistol, PlayerWeaponPose.Aim)] = AimPosePistol;
    }

    public void ApplyPose(GunType gunType, PlayerWeaponPose pose)
    {
        if (_poseTransforms.TryGetValue((gunType, pose), out var data))
        {
            transform.SetLocalPositionAndRotation(data.Position, Quaternion.Euler(data.EulerRotation));
        }
        else
        {
            Debug.LogWarning($"Chưa có pose {pose} cho loại súng {gunType}");
        }
    }
}
