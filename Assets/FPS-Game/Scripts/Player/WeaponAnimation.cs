using Unity.Netcode;
using UnityEngine;
public class WeaponAnimation : PlayerBehaviour
{
    public Animator animator;
    public bool Automatic;

    [HideInInspector]
    public bool IsShooting = false;

    void OnEnable()
    {
        animator.Play("Reload", 0, 0f);
        animator.Play("Shoot", 0, 0f);

        animator.Rebind();
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);

        IsShooting = false;
    }

    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        PlayerRoot.Events.OnReload += () =>
        {
            if (!IsShooting)
            {
                animator.SetBool("Reload", true);
            }
        };
    }

    bool _isPressed = false;

    void Update()
    {
        if (!IsOwner) return;
        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead()) return;

        if (Automatic)
        {
            if (PlayerRoot.PlayerAssetsInputs.shoot == true && !IsShooting && !PlayerRoot.PlayerReload.IsReloading)
            {
                animator.SetTrigger("Shoot");

                IsShooting = true;
            }
        }

        else
        {
            if (PlayerRoot.PlayerAssetsInputs.shoot == true && !IsShooting && !PlayerRoot.PlayerReload.IsReloading && _isPressed == false)
            {
                _isPressed = true;

                animator.SetTrigger("Shoot");

                IsShooting = true;
            }

            if (PlayerRoot.PlayerAssetsInputs.shoot == false) _isPressed = false;
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
    }
}
