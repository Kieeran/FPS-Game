using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class WeaponAnimation : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] PlayerReload _playerReload;

    public Animator animator;

    public override void OnNetworkSpawn()
    {
        _playerReload.reload += () =>
        {
            if (animator.GetBool("Reload_Mag") == false)
            {
                animator.SetBool("Reload_Mag", true);
            }
        };
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_playerAssetsInputs.shoot == true &&
        animator.GetBool("Shoot") == false && animator.GetBool("Reload_Single") == false)
        {
            animator.SetBool("Shoot", true);
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
    }

    public void DoneReloadMag()
    {
        animator.SetBool("Reload_Mag", false);
        animator.SetBool("Reload_Single", true);
    }
}
