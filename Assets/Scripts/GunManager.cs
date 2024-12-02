using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GunManager : NetworkBehaviour
{
    // [SerializeField] private GameObject primaryWeapon;
    public static GunManager Instance;

    // private void Start() {
    //     primaryWeapon.SetActive(true);
    // }

    private void Awake()
    {
        // if (Instance != null)
        //     Destroy(Instance);
        // else
        if (IsOwner) Instance = this;
    }
}