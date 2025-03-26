using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class Explosives : NetworkBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject explosiveEffectPrefab;
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    private float throwForce = 30f;

    private GameObject _currentGrenade;
    private Collider _collider;
    private NetworkObject networkObject;
    private bool _onCoolDown;

    private void Start()
    {
        // SpawnNewGrenade();
    }

    // public override void OnNetworkSpawn()
    // {
    //     SpawnNewGrenade();
    // }

    bool toggle = false;

    void Update()
    {
        if (playerAssetsInputs.shoot == true)
        {
            playerAssetsInputs.shoot = false;

            toggle = !toggle;

            if (toggle == true)
            {
                SpawnNewGrenade_ServerRPC();
            }

            else
            {
                // networkObject.Despawn(true);
                DestroyGrenade_ServerRPC();
            }

            // if (_onCoolDown == true) return;

            // _onCoolDown = true;

            // //_currentGrenade.transform.parent = null;
            // Rigidbody rb = _currentGrenade.AddComponent<Rigidbody>();

            // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            // _collider.enabled = true;

            // StartCoroutine(DestroyGrenade(_currentGrenade));
        }

        if (_currentGrenade != null)
            _currentGrenade.transform.position = transform.position;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnNewGrenade_ServerRPC()
    {
        _currentGrenade = Instantiate(grenadePrefab);

        networkObject = _currentGrenade.GetComponent<NetworkObject>();
        networkObject.Spawn();

        _collider = _currentGrenade.transform.GetComponent<Collider>();
        _collider.enabled = false;

        _onCoolDown = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyGrenade_ServerRPC()
    {
        networkObject.Despawn(true);
    }

    IEnumerator DestroyGrenade(GameObject grenade)
    {
        yield return new WaitForSeconds(2f);

        GameObject explosiveEffect = Instantiate(explosiveEffectPrefab);
        explosiveEffect.transform.position = grenade.transform.position;

        if (grenade != null)
        {
            // Destroy(grenade);
            networkObject.Despawn(false);
        }

        // SpawnNewGrenade();

        StartCoroutine(DestroyExplosiveEffect(explosiveEffect));
    }

    IEnumerator DestroyExplosiveEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3f);

        Destroy(effect);
    }
}