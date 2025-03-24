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
    [SerializeField] private float throwForce;

    private GameObject _currentGrenade;
    private Collider _collider;
    private bool _onCoolDown;

    public override void OnNetworkSpawn()
    {
        SpawnNewGrenade();
    }

    void Update()
    {
        if (playerAssetsInputs.shoot == true)
        {
            playerAssetsInputs.shoot = false;

            if (_onCoolDown == true) return;

            _onCoolDown = true;

            _currentGrenade.gameObject.transform.parent = null;
            Rigidbody rb = _currentGrenade.AddComponent<Rigidbody>();

            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            _collider.enabled = true;

            StartCoroutine(DestroyGrenade(_currentGrenade));
        }
    }

    private void SpawnNewGrenade()
    {
        _currentGrenade = Instantiate(grenadePrefab, transform);
        _currentGrenade.transform.localPosition = Vector3.zero;

        _collider = _currentGrenade.transform.GetComponent<Collider>();
        _collider.enabled = false;

        _onCoolDown = false;
    }

    IEnumerator DestroyGrenade(GameObject grenade)
    {
        yield return new WaitForSeconds(2f);

        GameObject explosiveEffect = Instantiate(explosiveEffectPrefab);
        explosiveEffect.transform.position = grenade.transform.position;

        if (grenade != null)
            Destroy(grenade.gameObject);

        SpawnNewGrenade();

        StartCoroutine(DestroyExplosiveEffect(explosiveEffect));
    }

    IEnumerator DestroyExplosiveEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3f);

        Destroy(effect);
    }
}