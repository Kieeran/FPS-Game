using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class Explosives : NetworkBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject explosiveEffectPrefab;
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    private float throwForce = 30f;

    [SerializeField] private GameObject _currentGrenade;
    [SerializeField] private Rigidbody _grenadeRb;
    private Collider _collider;
    private NetworkObject networkObject;
    private bool _onCoolDown = false;

    private void Start()
    {
        // SpawnNewGrenade();

        if (!IsOwner) return;

        _grenadeRb.isKinematic = true;
    }

    // public override void OnNetworkSpawn()
    // {
    //     SpawnNewGrenade();
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
        // _currentGrenade.SetActive(b);

        if (b == false)
        {
            _currentGrenade.transform.parent = null;
            _grenadeRb.isKinematic = false;
        }

        else
        {
            _currentGrenade.transform.SetParent(transform);
            _currentGrenade.transform.localPosition = Vector3.zero;

            _grenadeRb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (playerAssetsInputs.shoot == true)
        {
            playerAssetsInputs.shoot = false;

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

    [ServerRpc(RequireOwnership = false)]
    private void SpawnNewGrenade_ServerRPC()
    {
        _currentGrenade = Instantiate(grenadePrefab);

        networkObject = _currentGrenade.GetComponent<NetworkObject>();
        networkObject.Spawn();

        _collider = _currentGrenade.transform.GetComponent<Collider>();
        _collider.enabled = false;

        // _onCoolDown = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyGrenade_ServerRPC()
    {
        networkObject.Despawn(true);
    }

    IEnumerator DestroyGrenade(GameObject grenade)
    {
        yield return new WaitForSeconds(2f);
        _onCoolDown = false;
        DestroyGrenade_ServerRPC();

        // GameObject explosiveEffect = Instantiate(explosiveEffectPrefab);
        // explosiveEffect.transform.position = grenade.transform.position;

        // if (grenade != null)
        // {
        //     // Destroy(grenade);
        //     networkObject.Despawn(false);
        // }

        // SpawnNewGrenade();

        // StartCoroutine(DestroyExplosiveEffect(explosiveEffect));
    }

    IEnumerator DestroyExplosiveEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3f);

        Destroy(effect);
    }
}