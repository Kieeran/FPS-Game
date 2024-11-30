using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : ScriptableObject
{
    [field: SerializeField] public int WeaponID { get; private set; }
    [field: SerializeField] public float HeadDamage { get; private set; }
    [field: SerializeField] public float BodyDamage { get; private set; }
}