using System;
using System.Collections;
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

    [SerializeField]
    bool _isAttacking = false;
    [SerializeField]
    string _currentSlashType = "";
    [SerializeField] float _rightSlashDelay;

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
            CheckSlashHit();
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

    public void CheckSlashHit()
    {
        Vector3 slashBoundsSize = Vector3.zero;
        Vector3 slashOffset = Vector3.zero;

        Vector3 worldCenter = transform.TransformPoint(slashOffset);
        Vector3 worldHalfExtents = Vector3.Scale(slashBoundsSize, transform.lossyScale) * 0.5f;

        Collider[] hits = Physics.OverlapBox(worldCenter, worldHalfExtents);

        foreach (var hit in hits)
        {
            Debug.Log("Slash Hit: " + hit.name);
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