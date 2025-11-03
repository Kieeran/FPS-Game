using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class SniperAnimation : PlayerBehaviour
{
    public Animator animator;
    
    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();
        PlayerRoot.Events.OnReload += () =>
        {
            if (animator.GetBool("Reload_Mag") == false)
            {
                animator.SetBool("Reload_Mag", true);

                if (PlayerRoot.PlayerAim.ToggleAim == true)
                    PlayerRoot.PlayerCamera.UnAimScope();
            }
        };
    }

    void OnEnable()
    {
        animator.Rebind();
        animator.Update(0f);
    }

    bool _isPressed = false;

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.shoot == true &&
        animator.GetBool("Shoot") == false &&
        animator.GetBool("Reload_Single") == false &&
        _isPressed == false)
        {
            _isPressed = true;
            animator.SetBool("Shoot", true);

            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.UnAimScope();
        }

        if (PlayerRoot.PlayerAssetsInputs.shoot == false) _isPressed = false;
    }

    public void DoneShoot()
    {
        animator.SetBool("Shoot", false);
        animator.SetBool("Reload_Single", true);
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
}
