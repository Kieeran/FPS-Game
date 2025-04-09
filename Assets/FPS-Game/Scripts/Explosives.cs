using System.Collections;
using Mono.CSharp;
using PlayerAssets;
using Unity.Mathematics;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class Explosives : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] PlayerInventory _playerInventory;

    [SerializeField] GameObject _explosiveEffectPrefab;
    [SerializeField] GameObject _currentGrenade;
    Rigidbody _grenadeRb;
    Collider _collider;

    ClientNetworkTransform _clientNetworkTransform;
    bool _onCoolDown = false;

    float _throwForce;

    private void Start()
    {
        _clientNetworkTransform = _currentGrenade.GetComponent<ClientNetworkTransform>();
        _grenadeRb = _currentGrenade.GetComponent<Rigidbody>();
        _collider = _currentGrenade.GetComponent<Collider>();

        _grenadeRb.isKinematic = true;
        _collider.enabled = false;

        _throwForce = 20f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowGrenade_ServerRPC()
    {
        ThrowGrenade();
        ThrowGrenade_ClientRPC();
    }

    [ClientRpc]
    private void ThrowGrenade_ClientRPC()
    {
        ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        _currentGrenade.transform.parent = null;
        _grenadeRb.isKinematic = false;
        _collider.enabled = true;

        _grenadeRb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);

        Invoke(nameof(GrenadeExplodeState), 2f);
    }

    void GrenadeExplodeState()
    {
        _currentGrenade.SetActive(false);

        GameObject explodeEffect = Instantiate(_explosiveEffectPrefab);
        explodeEffect.transform.position = _currentGrenade.transform.position;

        StartCoroutine(DestroyExplodeEffect(explodeEffect));

        Invoke(nameof(GrenadeReturnState), 0.5f);
    }

    void GrenadeReturnState()
    {
        _currentGrenade.SetActive(true);

        _clientNetworkTransform.Interpolate = false;
        _grenadeRb.isKinematic = true;
        _collider.enabled = false;

        _currentGrenade.transform.SetParent(transform);
        _currentGrenade.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);

        _currentGrenade.transform.localScale = Vector3.one;

        Invoke(nameof(EnableInterpolation), 0.1f);
    }

    IEnumerator DestroyExplodeEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3f);

        Destroy(effect);
    }

    void EnableInterpolation()
    {
        if (_clientNetworkTransform != null)
        {
            _clientNetworkTransform.Interpolate = true;
            _onCoolDown = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_playerAssetsInputs.shoot == true)
        {
            _playerAssetsInputs.shoot = false;

            if (_onCoolDown == true) return;

            _onCoolDown = true;

            _playerInventory.UpdatecurrentMagazineAmmo();

            ThrowGrenade_ServerRPC();
        }
    }
}