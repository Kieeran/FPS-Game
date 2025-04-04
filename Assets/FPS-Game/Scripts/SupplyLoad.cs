using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyLoad : MonoBehaviour
{
    [SerializeField] int _totalSupplies;
    [SerializeField] int _capacity;

    public bool IsOutOfSupplies() { return _totalSupplies <= 0; }
}
