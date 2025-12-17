using Unity.Netcode;
using UnityEngine;
public class WeaponAnimation : PlayerBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Gun gun;

    float shootSpeed;

    void OnEnable()
    {
        animator.Play("Reload", 0, 0f);
        animator.Play("Shoot", 0, 0f);

        animator.Rebind();
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);

        SetAnimationSpeed();
    }

    void Start()
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Shoot")
            {
                shootSpeed = clip.length / gun.FireCoolDown;
                break;
            }
        }
        SetAnimationSpeed();
    }

    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        PlayerRoot.Events.OnReload += () =>
        {
            animator.SetBool("Reload", true);
        };

        PlayerRoot.Events.OnGunShoot += () =>
        {
            animator.SetTrigger("Shoot");
        };
    }

    public void OnDoneReload()
    {
        if (!IsOwner) return;

        animator.SetBool("Reload", false);
        PlayerRoot.Events.InvokeOnDoneReload();
    }

    void SetAnimationSpeed()
    {
        animator.SetFloat("ShootSpeed", shootSpeed);
    }
}
