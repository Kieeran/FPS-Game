using Unity.Netcode;
using UnityEngine;
public class WeaponAnimation : PlayerBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Gun gun;

    string shootAnimName = "Shoot";
    string reloadAnimName = "Reload";
    public float shootSpeed;
    public float reloadSpeed;

    void OnEnable()
    {
        animator.Play("Reload", 0, 0f);
        animator.Play("Shoot", 0, 0f);

        animator.Rebind();
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);

        SetAnimationSpeed();
    }

    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        PlayerRoot.Events.OnReload += () =>
        {
            animator.SetTrigger("Reload");
        };

        PlayerRoot.Events.OnGunShoot += () =>
        {
            animator.SetTrigger("Shoot");
        };

        CalculateAnimSpeeds();
        SetAnimationSpeed();
    }

    void SetAnimationSpeed()
    {
        animator.SetFloat("ShootAniSpeed", shootSpeed);
        animator.SetFloat("ReloadAniSpeed", reloadSpeed);
    }

    void CalculateAnimSpeeds()
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == shootAnimName)
            {
                shootSpeed = clip.length / gun.FireCoolDown;
            }

            if (clip.name == reloadAnimName)
            {
                reloadSpeed = clip.length / gun.ReloadCoolDown;
            }
        }
    }
}
