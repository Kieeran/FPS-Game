using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyLoad : MonoBehaviour
{
    [SerializeField] public int InitSupplies;
    [SerializeField] public int Capacity;

    [HideInInspector]
    public int CurrentMagazineAmmo;
    [HideInInspector]
    public int TotalSupplies;

    void Start()
    {
        CurrentMagazineAmmo = Capacity;
        TotalSupplies = InitSupplies;
    }

    public bool IsMagazineEmpty() { return CurrentMagazineAmmo <= 0; }
    public bool IsTotalSuppliesEmpty() { return TotalSupplies <= 0; }

    public void RefillAmmo()
    {
        int currentTotalSupplies = TotalSupplies + CurrentMagazineAmmo;
        int initTotalSupplies = InitSupplies + Capacity;

        TotalSupplies += initTotalSupplies - currentTotalSupplies;
    }
}
