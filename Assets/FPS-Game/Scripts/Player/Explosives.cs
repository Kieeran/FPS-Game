using System.Collections;
using Mono.CSharp;
using PlayerAssets;
using Unity.Mathematics;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class Explosives : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }

    [SerializeField] GameObject _explosiveEffectPrefab;
    [SerializeField] GameObject _currentGrenade;
    Rigidbody _grenadeRb;
    Collider _collider;
    SupplyLoad _supplyLoad;

    ClientNetworkTransform _clientNetworkTransform;
    bool _onCoolDown = false;

    float _throwForce;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    private void Start()
    {
        _supplyLoad = GetComponent<SupplyLoad>();

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

        _currentGrenade.SetActive(false);
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

    [ServerRpc(RequireOwnership = false)]
    void EnableCurrentGrenade_ServerRPC()
    {
        EnableCurrentGrenade();
        EnableCurrentGrenade_ClientRPC();
    }

    [ClientRpc]
    void EnableCurrentGrenade_ClientRPC()
    {
        EnableCurrentGrenade();
    }

    void EnableCurrentGrenade()
    {
        _currentGrenade.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        if (_supplyLoad.IsMagazineEmpty()) return;
        if (PlayerRoot.PlayerReload.GetIsReloading()) return;

        if (_currentGrenade.activeSelf == false)
        {
            EnableCurrentGrenade_ServerRPC();
        }

        if (PlayerRoot.PlayerAssetsInputs.shoot == true)
        {
            PlayerRoot.PlayerAssetsInputs.shoot = false;

            if (_onCoolDown == true) return;

            _onCoolDown = true;

            PlayerRoot.PlayerInventory.UpdatecurrentMagazineAmmo();

            ThrowGrenade_ServerRPC();
        }
    }
}