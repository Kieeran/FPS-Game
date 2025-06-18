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
    [SerializeField] Vector3 _leftSlashBoundsSize = new(0.15f, 0.15f, 0.5f);
    [SerializeField] Vector3 _leftSlashOffset = new(-0.56f, 0.24f, 0f);

    [Header("Right Slash")]
    public bool EnableRightSlashBBVisual;
    [SerializeField] Vector3 _rightSlashBoundsSize = new(0.15f, 0.15f, 0.75f);
    [SerializeField] Vector3 _rightSlashOffset = new(-0.56f, 0.24f, 0.125f);

    public bool isAttacking = false;
    public string CurrentSlashType = "";

    public Action OnLeftSlash_1;
    public Action OnLeftSlash_2;
    public Action OnRightSlash;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();

        MeleeAnimation.OnDoneSlash += () =>
        {
            CheckComboChain();
        };
    }

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;
        if (isAttacking) return;

        if (PlayerRoot.PlayerAssetsInputs.shoot && CurrentSlashType == "")
        {
            isAttacking = true;
            CurrentSlashType = "Left 1";
            OnLeftSlash_1?.Invoke();
        }

        else if (PlayerRoot.PlayerAssetsInputs.rightSlash)
        {
            isAttacking = true;
            CurrentSlashType = "";
            OnRightSlash?.Invoke();
        }
    }

    void CheckComboChain()
    {
        if (PlayerRoot.PlayerAssetsInputs.shoot && CurrentSlashType == "")
        {
            CurrentSlashType = "Left 1";
            OnLeftSlash_1?.Invoke();
            return;
        }

        else if (PlayerRoot.PlayerAssetsInputs.shoot && CurrentSlashType == "Left 1")
        {
            CurrentSlashType = "";
            OnLeftSlash_2?.Invoke();
            return;
        }

        else if (PlayerRoot.PlayerAssetsInputs.rightSlash)
        {
            CurrentSlashType = "";
            OnRightSlash?.Invoke();
            return;
        }

        CurrentSlashType = "";
        isAttacking = false;
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