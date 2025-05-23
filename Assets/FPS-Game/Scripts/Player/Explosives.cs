using System.Collections;
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

    Vector3 originPosGrenade;
    Quaternion originRotGrenade;
    Vector3 originScaGrenade;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    private void Start()
    {
        _supplyLoad = GetComponent<SupplyLoad>();

        if (_currentGrenade != null)
        {
            _clientNetworkTransform = _currentGrenade.GetComponent<ClientNetworkTransform>();
            _grenadeRb = _currentGrenade.GetComponent<Rigidbody>();
            _collider = _currentGrenade.GetComponent<Collider>();

            originPosGrenade = _currentGrenade.transform.localPosition;
            originRotGrenade = _currentGrenade.transform.localRotation;
            originScaGrenade = _currentGrenade.transform.localScale;

            _grenadeRb.isKinematic = true;
            _collider.enabled = false;
        }
        else
        {
            Debug.LogError("Current grenade is not assigned!", this);
        }

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

        Invoke(nameof(GrenadeExplode), 2f);
    }

    void GrenadeExplode()
    {
        _currentGrenade.SetActive(false);

        GameObject explodeEffect = Instantiate(_explosiveEffectPrefab);
        explodeEffect.transform.position = _currentGrenade.transform.position;

        StartCoroutine(DestroyExplodeEffect(explodeEffect));

        Invoke(nameof(GrenadeReturn), 0.5f);
    }

    void GrenadeReturn()
    {
        _currentGrenade.SetActive(true);

        // _clientNetworkTransform.Interpolate = false;
        _grenadeRb.isKinematic = true;
        _collider.enabled = false;

        _currentGrenade.transform.SetParent(transform);
        ResetGrenadeTransform();

        _currentGrenade.SetActive(false);
        Invoke(nameof(EnableInterpolation), 0.1f);
    }

    void ResetGrenadeTransform()
    {
        _currentGrenade.transform.SetLocalPositionAndRotation(originPosGrenade, originRotGrenade);
        _currentGrenade.transform.localScale = originScaGrenade;
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
            // _clientNetworkTransform.Interpolate = true;
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