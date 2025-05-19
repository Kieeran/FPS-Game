
using Unity.Netcode;
using UnityEngine;
public class AniTest : NetworkBehaviour
{
    public Animator animator;
    public bool Automatic;

    public bool IsShooting = false;
    public bool IsReloading = false;

    void Update()
    {
        if (!IsOwner) return;

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
        if (!IsOwner) return;

        IsShooting = false;
    }

    public void OnDoneReload()
    {
        if (!IsOwner) return;

        animator.SetBool("Reload", false);
        IsReloading = false;
    }
}
