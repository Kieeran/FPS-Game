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
    private bool _isInitialized = false;

    void Start()
    {
        // CurrentMagazineAmmo = Capacity;
        // TotalSupplies = InitSupplies;

        if (!_isInitialized)
        {
            CurrentMagazineAmmo = Capacity;
            TotalSupplies = InitSupplies;
            _isInitialized = true;
        }
    }

    public void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            CurrentMagazineAmmo = Capacity;
            TotalSupplies = InitSupplies;
            _isInitialized = true;
        }
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
