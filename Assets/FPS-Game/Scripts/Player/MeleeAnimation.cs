using System;
using Unity.Netcode;
using UnityEngine;

public class MeleeAnimation : PlayerBehaviour
{
    public Animator Animator;

    public override void InitializeAwake()
    {
        base.InitializeAwake();
        PlayerRoot.Events.OnLeftSlash_1 += () =>
        {
            Animator.SetTrigger("LeftSlash_1");
        };

        PlayerRoot.Events.OnLeftSlash_2 += () =>
        {
            Animator.SetTrigger("LeftSlash_2");
        };

        PlayerRoot.Events.OnRightSlash += () =>
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
        PlayerRoot.Events.InvokeOnDoneSlash();
    }

    public void CheckSlashHit()
    {
        PlayerRoot.Events.InvokeOnCheckSlashHit();
    }
}