using System;
using Unity.Netcode;
using UnityEngine;

public class MeleeAnimation : PlayerBehaviour
{
    public Animator Animator;
    public Melees Melees;

    public Action OnDoneSlash;
    public Action OnCheckSlashHit;

    public override void InitializeAwake()
    {
        base.InitializeAwake();
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

    public void CheckSlashHit()
    {
        OnCheckSlashHit?.Invoke();
    }
}