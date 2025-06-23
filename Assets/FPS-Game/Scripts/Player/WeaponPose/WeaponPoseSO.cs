using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Weapon/Weapon Pose Per Gun")]
public class WeaponPoseSO : ScriptableObject
{
    public GunType GunType;

    [Tooltip("Danh sách các pose của loại súng này")]
    public List<WeaponTransformData> Poses = new();

    private Dictionary<PlayerWeaponPose, WeaponTransformData> _lookup;

    public void BuildLookup()
    {
        _lookup = new();

        foreach (var data in Poses)
        {
            _lookup[data.PoseType] = data;
        }
    }

    public bool TryGetPose(PlayerWeaponPose poseType, out WeaponTransformData result)
    {
        if (_lookup == null) BuildLookup();

        if (!_lookup.ContainsKey(poseType))
        {
            result = default;
            return false;
        }

        return _lookup.TryGetValue(poseType, out result);
    }
}