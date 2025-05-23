using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniTest : MonoBehaviour
{
    public Animator animator;
    public bool Automatic;

    public bool IsShooting = false;
    public bool IsReloading = false;

    void Update()
    {
        if (Automatic)
        {
            if (Input.GetMouseButton(0) && !IsShooting && !IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
            }

            if (Input.GetKeyDown(KeyCode.R) && !IsShooting && !IsReloading)
            {
                animator.SetBool("Reload", true);

                IsReloading = true;
            }
        }

        else
        {
            if (Input.GetMouseButtonDown(0) && !IsShooting && !IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
            }

            if (Input.GetKeyDown(KeyCode.R) && !IsShooting && !IsReloading)
            {
                animator.SetBool("Reload", true);

                IsReloading = true;
            }
        }
    }

    public void OnDoneShoot()
    {
        IsShooting = false;
    }

    public void OnDoneReload()
    {
        animator.SetBool("Reload", false);
        IsReloading = false;
    }
}
