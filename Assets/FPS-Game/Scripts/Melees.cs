using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Melees : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public MeleeAnimation MeleeAnimation;

    [Header("Left Slash")]
    public bool EnableLeftSlashBBVisual;
    [SerializeField] Vector3 _leftSlashBoundsSize = new(1f, 0.15f, 0.5f);
    [SerializeField] Vector3 _leftSlashOffset = new(-0.56f, 0.24f, 0f);

    [Header("Right Slash")]
    public bool EnableRightSlashBBVisual;
    [SerializeField] Vector3 _rightSlashBoundsSize = new(0.15f, 0.15f, 1f);
    [SerializeField] Vector3 _rightSlashOffset = new(-0.56f, 0.24f, 0.25f);

    public Action OnLeftSlash_1;
    public Action OnLeftSlash_2;
    public Action OnRightSlash;

    [SerializeField] float _rightSlashDelay = 0.3f;
    [SerializeField] float _meleeLeftSlashDamage = 0.1f;
    [SerializeField] float _meleeRightSlashDamage = 0.3f;

    bool _isAttacking = false;
    string _currentSlashType = "";

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();

        MeleeAnimation.OnDoneSlash += () =>
        {
            if (_currentSlashType == "Right")
                Invoke(nameof(CheckComboChain), _rightSlashDelay);
            else
                CheckComboChain();
        };

        MeleeAnimation.OnCheckSlashHit += () =>
        {
            CheckSlashHit_ServerRPC(_currentSlashType, OwnerClientId);
        };
    }

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;
        if (_isAttacking) return;

        if (PlayerRoot.PlayerAssetsInputs.shoot && _currentSlashType == "")
        {
            _isAttacking = true;
            _currentSlashType = "Left 1";
            OnLeftSlash_1?.Invoke();
        }

        else if (PlayerRoot.PlayerAssetsInputs.rightSlash)
        {
            _isAttacking = true;
            _currentSlashType = "Right";
            OnRightSlash?.Invoke();
        }
    }

    void CheckComboChain()
    {
        if (PlayerRoot.PlayerAssetsInputs.shoot &&
        (_currentSlashType == "Left 2" ||
        _currentSlashType == "Right"))
        {
            _currentSlashType = "Left 1";
            OnLeftSlash_1?.Invoke();
            return;
        }

        else if (PlayerRoot.PlayerAssetsInputs.shoot && _currentSlashType == "Left 1")
        {
            _currentSlashType = "Left 2";
            OnLeftSlash_2?.Invoke();
            return;
        }

        else if (PlayerRoot.PlayerAssetsInputs.rightSlash)
        {
            _currentSlashType = "Right";
            OnRightSlash?.Invoke();
            return;
        }

        _currentSlashType = "";
        _isAttacking = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckSlashHit_ServerRPC(string currentSlashType, ulong clientId)
    {
        Vector3 slashBoundsSize, slashOffset;
        float damage;

        switch (currentSlashType)
        {
            case "Left 1":
            case "Left 2":
                slashBoundsSize = _leftSlashBoundsSize;
                slashOffset = _leftSlashOffset;
                damage = _meleeLeftSlashDamage;

                break;
            case "Right":
                slashBoundsSize = _rightSlashBoundsSize;
                slashOffset = _rightSlashOffset;
                damage = _meleeRightSlashDamage;

                break;
            default:
                Debug.Log("_currentSlashType error");
                return;
        }

        Vector3 worldCenter = transform.TransformPoint(slashOffset);
        Vector3 worldHalfExtents = Vector3.Scale(slashBoundsSize, transform.lossyScale) * 0.5f;

        Collider[] hitColliders = Physics.OverlapBox(worldCenter, worldHalfExtents);

        HashSet<ulong> affectedClientIds = new(); // Tự động loại trùng

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Weapon")) continue;

            Transform root = hitCollider.transform.root;
            if (root.CompareTag("Player"))
            {
                if (root.TryGetComponent<NetworkObject>(out var netObj))
                {
                    ulong targetClientID = netObj.OwnerClientId;

                    if (targetClientID == clientId) continue;

                    if (affectedClientIds.Add(targetClientID)) // Add trả về false nếu clientId đã có
                    {
                        netObj.GetComponent<PlayerTakeDamage>().TakeDamage(damage, "Headshot", targetClientID, clientId);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // Vẽ vùng chém trái trong local space
        if (EnableLeftSlashBBVisual)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_leftSlashOffset, _leftSlashBoundsSize);
        }

        // Vẽ vùng chém phải trong local space
        if (EnableRightSlashBBVisual)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_rightSlashOffset, _rightSlashBoundsSize);
        }

        // Khôi phục lại ma trận cũ để tránh ảnh hưởng đến các gizmo khác
        Gizmos.matrix = oldMatrix;
    }
}