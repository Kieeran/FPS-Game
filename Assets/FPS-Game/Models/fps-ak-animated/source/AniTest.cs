using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniTest : MonoBehaviour
{
    public Animator animator;

    public bool IsShooting = false;
    public bool IsReloading = false;

    void Update()
    {
        if (Input.GetMouseButton(0) && !IsShooting && !IsReloading)
        {
            animator.SetTrigger("AK47_Shoot");

            IsShooting = true;
        }

        if (Input.GetKeyDown(KeyCode.R) && !IsShooting && !IsReloading)
        {
            animator.SetBool("AK47_Reload", true);

            IsReloading = true;
        }
    }

    public void OnDoneShoot()
    {
        IsShooting = false;
    }

    public void OnDoneReload()
    {
        animator.SetBool("AK47_Reload", false);
        IsReloading = false;
    }
}
