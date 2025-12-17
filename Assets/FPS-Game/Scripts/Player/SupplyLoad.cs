using UnityEngine;

public class SupplyLoad : MonoBehaviour
{
    public int Capacity;
    public int InitSupplies;
    public int CurrentMagazineAmmo;
    public int TotalSupplies;
    private bool _isInitialized = false;

    void Start()
    {
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
