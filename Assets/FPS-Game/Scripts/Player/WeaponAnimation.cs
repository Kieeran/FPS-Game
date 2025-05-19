using PlayerAssets;
using Unity.Netcode;
using UnityEngine;
public class WeaponAnimation : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] PlayerReload _playerReload;
    [SerializeField] PlayerAim _playerAim;
    public Animator animator;
    public bool Automatic;

    public bool IsShooting = false;
    public bool IsReloading = false;

    public override void OnNetworkSpawn()
    {
        _playerReload.reload += () =>
        {
            if (!IsShooting && !IsReloading)
            {
                animator.SetBool("Reload", true);

                IsReloading = true;
            }
        };
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Automatic)
        {
            if (_playerAssetsInputs.shoot == true && !IsShooting && !IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
            }

            // if (Input.GetKeyDown(KeyCode.R) && !IsShooting && !IsReloading)
            // {
            //     animator.SetBool("Reload", true);

            //     IsReloading = true;
            // }
        }

        else
        {
            if (_playerAssetsInputs.shoot == true && !IsShooting && !IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
            }

            // if (Input.GetKeyDown(KeyCode.R) && !IsShooting && !IsReloading)
            // {
            //     animator.SetBool("Reload", true);

            //     IsReloading = true;
            // }
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
