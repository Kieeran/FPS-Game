using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class SniperAnimation : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }

    public Animator animator;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    public override void OnNetworkSpawn()
    {
        PlayerRoot.PlayerReload.reload += () =>
        {
            if (animator.GetBool("Reload_Mag") == false)
            {
                animator.SetBool("Reload_Mag", true);

                if (PlayerRoot.PlayerAim.ToggleAim == true)
                    PlayerRoot.PlayerCamera.UnAimScope();
            }
        };
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.shoot == true &&
        animator.GetBool("Shoot") == false && animator.GetBool("Reload_Single") == false)
        {
            animator.SetBool("Shoot", true);

            if (PlayerRoot.PlayerAim.ToggleAim == true)
                PlayerRoot.PlayerCamera.UnAimScope();
        }
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
