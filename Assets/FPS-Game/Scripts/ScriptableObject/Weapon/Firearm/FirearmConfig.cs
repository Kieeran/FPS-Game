using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmConfig : WeaponConfig
{
    [field: SerializeField] public float FireCoolDown { get; private set; }
    [field: SerializeField] public int MagazineCapacity { get; private set; }
}