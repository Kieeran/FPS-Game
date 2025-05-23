using Unity.Netcode;
using UnityEngine;
public class WeaponAnimation : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public Animator animator;
    public bool Automatic;

    public bool IsShooting = false;
    public bool IsReloading = false;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    public override void OnNetworkSpawn()
    {
        PlayerRoot.PlayerReload.reload += () =>
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
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        if (Automatic)
        {
            if (PlayerRoot.PlayerAssetsInputs.shoot == true && !IsShooting && !IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
            }
        }

        else
        {
            if (PlayerRoot.PlayerAssetsInputs.shoot == true && !IsShooting && !IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
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
