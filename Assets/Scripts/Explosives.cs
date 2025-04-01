using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Mathematics;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class Explosives : NetworkBehaviour
{
    [SerializeField] GameObject _explosiveEffectPrefab;
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] GameObject _currentGrenade;
    [SerializeField] Rigidbody _grenadeRb;
    Collider _collider;

    ClientNetworkTransform _clientNetworkTransform;
    bool _onCoolDown = false;

    float _throwForce = 30f;

    private void Start()
    {
        _clientNetworkTransform = _currentGrenade.GetComponent<ClientNetworkTransform>();

        _grenadeRb.isKinematic = true;
    }

    // public override void OnNetworkSpawn()
    // {
    //     
    // }

    bool toggle = false;

    [ServerRpc(RequireOwnership = false)]
    private void RequestSetActiveGrenade_ServerRPC(bool b)
    {
        SetActiveGrenade(b);
        RequestSetActiveGrenade_ClientRPC(b);
    }

    [ClientRpc]
    private void RequestSetActiveGrenade_ClientRPC(bool b)
    {
        SetActiveGrenade(b);
    }

    private void SetActiveGrenade(bool b)
    {
        if (b == false)
        {
            _currentGrenade.transform.parent = null;
            _grenadeRb.isKinematic = false;
        }

        else
        {
            _clientNetworkTransform.Interpolate = false;
            _currentGrenade.transform.SetParent(transform);
            _currentGrenade.transform.localPosition = Vector3.zero;
            _currentGrenade.transform.localRotation = quaternion.identity;

            _grenadeRb.isKinematic = true;

            Invoke(nameof(_enableInterpolation), 0.1f);
        }
    }

    void _enableInterpolation()
    {
        if (_clientNetworkTransform != null)
            _clientNetworkTransform.Interpolate = true;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_playerAssetsInputs.shoot == true)
        {
            _playerAssetsInputs.shoot = false;

            RequestSetActiveGrenade_ServerRPC(toggle);

            toggle = !toggle;

            // if (toggle == true)
            // {
            //     SpawnNewGrenade_ServerRPC();
            // }

            // else
            // {
            //     // networkObject.Despawn(true);
            //     DestroyGrenade_ServerRPC();
            // }

            // if (_onCoolDown == true) return;

            // _onCoolDown = true;

            // SpawnNewGrenade_ServerRPC();
            // StartCoroutine(DestroyGrenade(_currentGrenade));

            // //_currentGrenade.transform.parent = null;
            // Rigidbody rb = _currentGrenade.AddComponent<Rigidbody>();

            // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            // _collider.enabled = true;

            // StartCoroutine(DestroyGrenade(_currentGrenade));
        }

        // if (_currentGrenade != null)
        //     _currentGrenade.transform.position = transform.position;
    }
}