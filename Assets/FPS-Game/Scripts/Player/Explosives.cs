using System.Collections;
using System.Collections.Generic;
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


    public bool EnableRadiusVisual;
    [SerializeField] float _throwForce;
    [SerializeField] float _explosionRadius;
    [SerializeField] float _outerRadius;
    [SerializeField] float _middleRadius;
    [SerializeField] float _innerRadius;

    [SerializeField] float _outerDamage;
    [SerializeField] float _middleDamage;
    [SerializeField] float _innerDamage;

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

            _clientNetworkTransform.enabled = false;
        }

        else
        {
            Debug.LogError("Current grenade is not assigned!", this);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _clientNetworkTransform.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowGrenade_ServerRPC(ulong throwerClientId)
    {
        // ThrowGrenade();
        ThrowGrenade_ClientRPC(throwerClientId);
    }

    [ClientRpc]
    private void ThrowGrenade_ClientRPC(ulong throwerClientId)
    {
        ThrowGrenade(throwerClientId);
    }

    private void ThrowGrenade(ulong throwerClientId)
    {
        _currentGrenade.transform.parent = null;
        _grenadeRb.isKinematic = false;
        _collider.enabled = true;

        _grenadeRb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);

        StartCoroutine(GrenadeExplode(throwerClientId));
    }

    IEnumerator GrenadeExplode(ulong throwerClientId)
    {
        yield return new WaitForSeconds(2f);

        if (IsServer) ScanForTargets_ServerRPC(throwerClientId);

        _currentGrenade.SetActive(false);

        GameObject explodeEffect = Instantiate(_explosiveEffectPrefab);
        explodeEffect.transform.position = _currentGrenade.transform.position;

        StartCoroutine(DestroyExplodeEffect(explodeEffect));

        Invoke(nameof(GrenadeReturn), 0.5f);
    }

    [ServerRpc(RequireOwnership = false)]
    void ScanForTargets_ServerRPC(ulong throwerClientId)
    {
        Collider[] hitColliders = Physics.OverlapSphere(_currentGrenade.transform.position, _explosionRadius);

        HashSet<ulong> affectedClientIds = new(); // Tự động loại trùng

        foreach (var hitCollider in hitColliders)
        {
            Transform root = hitCollider.transform.root;

            if (root.CompareTag("Player"))
            {
                if (root.TryGetComponent<NetworkObject>(out var netObj))
                {
                    ulong clientId = netObj.OwnerClientId;

                    if (affectedClientIds.Add(clientId)) // Add trả về false nếu clientId đã có
                    {
                        float damage = GetDamageByDistance(netObj.transform.position);
                        netObj.GetComponent<PlayerTakeDamage>().TakeDamage(damage, "Headshot", clientId, throwerClientId);
                    }
                }
            }
        }
    }

    float GetDamageByDistance(Vector3 playerPos)
    {
        float distance = Vector3.Distance(_currentGrenade.transform.position, playerPos);

        if (distance < _outerRadius && distance >= _middleRadius)
        {
            Debug.Log("In outer range");
            return _outerDamage;
        }

        else if (distance < _middleRadius && distance >= _innerRadius)
        {
            Debug.Log("In middle range");
            return _middleDamage;
        }

        else if (distance < _innerRadius)
        {
            Debug.Log("In inner range");
            return _innerDamage;
        }

        else return 0;
    }

    void GrenadeReturn()
    {
        // _currentGrenade.SetActive(true);

        _clientNetworkTransform.Interpolate = false;
        _grenadeRb.isKinematic = true;
        _collider.enabled = false;

        _currentGrenade.transform.SetParent(transform);
        ResetGrenadeTransform();

        // _currentGrenade.SetActive(false);
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
            _clientNetworkTransform.Interpolate = true;
            _onCoolDown = false;
        }

        _currentGrenade.SetActive(true);
    }

    // [ServerRpc(RequireOwnership = false)]
    // void EnableCurrentGrenade_ServerRPC()
    // {
    //     EnableCurrentGrenade();
    //     EnableCurrentGrenade_ClientRPC();
    // }

    // [ClientRpc]
    // void EnableCurrentGrenade_ClientRPC()
    // {
    //     EnableCurrentGrenade();
    // }

    // void EnableCurrentGrenade()
    // {
    //     _currentGrenade.gameObject.SetActive(true);
    // }

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        if (_supplyLoad.IsMagazineEmpty()) return;
        if (PlayerRoot.PlayerReload.GetIsReloading()) return;

        // if (_currentGrenade.activeSelf == false)
        // {
        //     EnableCurrentGrenade_ServerRPC();
        // }

        if (PlayerRoot.PlayerAssetsInputs.shoot == true)
        {
            PlayerRoot.PlayerAssetsInputs.shoot = false;

            if (_onCoolDown == true) return;

            _onCoolDown = true;

            PlayerRoot.PlayerInventory.UpdatecurrentMagazineAmmo();

            ThrowGrenade_ServerRPC(OwnerClientId);
        }
    }

    void OnDrawGizmos()
    {
        if (!EnableRadiusVisual) return;

        Color colorGreen = Color.green;
        Color colorYellow = Color.yellow;
        Color colorRed = Color.red;

        Gizmos.color = colorGreen;
        Gizmos.DrawWireSphere(_currentGrenade.transform.position, _outerRadius);

        Gizmos.color = colorYellow;
        Gizmos.DrawWireSphere(_currentGrenade.transform.position, _middleRadius);

        Gizmos.color = colorRed;
        Gizmos.DrawWireSphere(_currentGrenade.transform.position, _innerRadius);
    }
}