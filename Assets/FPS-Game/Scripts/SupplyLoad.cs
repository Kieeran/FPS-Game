using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyLoad : MonoBehaviour
{
    [SerializeField] int _totalSupplies;
    [SerializeField] int _capacity;

    public int GetTotalSupplies() { return _totalSupplies; }
    public int GetCapacity() { return _capacity; }

    public bool IsOutOfSupplies() { return _totalSupplies <= 0; }
}
