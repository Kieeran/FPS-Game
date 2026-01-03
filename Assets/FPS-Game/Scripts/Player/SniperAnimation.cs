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

        PlayerRoot.Events.OnDoneReload += () =>
        {
            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.AimScope();
        };

        PlayerRoot.Events.OnGunShoot += () =>
        {
            SetAnimationSpeed(shootCycleAniSpeed);
            animator.SetTrigger("Shoot");

            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.UnAimScope();
        };

        PlayerRoot.Events.OnDoneGunShoot += () =>
        {
            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.AimScope();
        };
    }

    void OnEnable()
    {
        animator.Rebind();
        animator.Update(0f);
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
        fullReloadAniSpeed = fullReloadAniDuration / gun.ReloadCoolDown;
    }

    void SetAnimationSpeed(float multiplier)
    {
        animator.SetFloat("SpeedMultiplier", multiplier);
    }
}
