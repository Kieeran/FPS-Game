using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class SniperAnimation : PlayerBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Gun gun;
    string shootAnimName = "Shoot";
    string boltActionAnimName = "Reload_Single";
    string reloadAnimName = "Reload_Mag";
    float shootCycleAniSpeed;
    float fullReloadAniSpeed;

    void Start()
    {
        CalculateAnimSpeeds();
    }

    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        PlayerRoot.Events.OnReload += () =>
        {
            SetAnimationSpeed(fullReloadAniSpeed);
            animator.SetTrigger("Reload");
            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.UnAimScope();
        };

        PlayerRoot.Events.OnGunShoot += () =>
        {
            SetAnimationSpeed(shootCycleAniSpeed);
            animator.SetTrigger("Shoot");

            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.UnAimScope();
        };
    }

    void OnEnable()
    {
        animator.Rebind();
        animator.Update(0f);
    }

    public void DoneReloadSingle()
    {
        animator.SetBool("Reload_Single", false);

        if (PlayerRoot.PlayerAim.ToggleAim == true)
            PlayerRoot.PlayerCamera.AimScope();
    }

    public void DoneReloadMag()
    {
        animator.SetBool("Reload_Mag", false);
        animator.SetBool("Reload_Single", true);
    }

    void CalculateAnimSpeeds()
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        float shootAnimDuration = 0f;
        float boltActionAnimDuration = 0f;
        float reloadAnimDuration = 0f;

        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == shootAnimName)
            {
                shootAnimDuration = clip.length;
            }

            if (clip.name == boltActionAnimName)
            {
                boltActionAnimDuration = clip.length;
            }

            if (clip.name == reloadAnimName)
            {
                reloadAnimDuration = clip.length;
            }
        }

        float shootCycleAniDuration = shootAnimDuration + boltActionAnimDuration;
        float fullReloadAniDuration = reloadAnimDuration + boltActionAnimDuration;

        shootCycleAniSpeed = shootCycleAniDuration / gun.FireCoolDown;
        fullReloadAniSpeed = fullReloadAniDuration / gun.FireCoolDown;
    }

    void SetAnimationSpeed(float multiplier)
    {
        animator.SetFloat("SpeedMultiplier", multiplier);
    }
}
