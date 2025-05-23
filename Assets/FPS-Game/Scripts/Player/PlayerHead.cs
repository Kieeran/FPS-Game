using Unity.Netcode;
using UnityEngine;

public class PlayerHead : NetworkBehaviour
{
    public Rigidbody Rb { get; private set; }

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }
}