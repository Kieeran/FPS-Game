using UnityEngine;

public enum PlayerWeaponPose
{
    Idle,
    Aim,
}

[System.Serializable]
public struct WeaponTransformData
{
    public PlayerWeaponPose PoseType;
    public Vector3 Position;
    // public Vector3 EulerRotation;

    public WeaponTransformData(PlayerWeaponPose poseType, Vector3 position/*, Vector3 eulerRotation*/)
    {
        PoseType = poseType;
        Position = position;
        // EulerRotation = eulerRotation;
    }
}

[System.Serializable]
public struct WeaponPoseSet
{
    public GunType GunType;
    public WeaponTransformData IdlePose;
}