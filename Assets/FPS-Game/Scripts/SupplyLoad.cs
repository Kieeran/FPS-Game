using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyLoad : MonoBehaviour
{
    [SerializeField] public int TotalSupplies;
    [SerializeField] public int Capacity;

    public int CurrentMagazineAmmo;

    public bool IsMagazineEmpty() { return CurrentMagazineAmmo <= 0; }
    public bool IsTotalSuppliesEmpty() { return TotalSupplies <= 0; }
}
