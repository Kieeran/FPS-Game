using System;
using Unity.Netcode;
using UnityEngine;

public class MeleeAnimation : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public Animator Animator;
    public Melees Melees;

    public Action OnDoneSlash;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();

        Melees.OnLeftSlash_1 += () =>
        {
            Animator.SetTrigger("LeftSlash_1");
        };

        Melees.OnLeftSlash_2 += () =>
        {
            Animator.SetTrigger("LeftSlash_2");
        };

        Melees.OnRightSlash += () =>
        {
            Animator.SetTrigger("RightSlash");
        };
    }

    void OnEnable()
    {
        Animator.Rebind();
        Animator.Update(0f);
    }

    public void DoneSlash()
    {
        OnDoneSlash?.Invoke();
    }
}