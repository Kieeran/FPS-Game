using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class WeaponAnimation : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] PlayerCamera _playerCamera;
    [SerializeField] PlayerReload _playerReload;
    [SerializeField] PlayerAim _playerAim;

    public Animator animator;

    public override void OnNetworkSpawn()
    {
        _playerReload.reload += () =>
        {
            if (animator.GetBool("Reload_Mag") == false)
            {
                animator.SetBool("Reload_Mag", true);

                if (_playerAim.ToggleAim == true)
                    _playerCamera.UnAimScope();
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

            if (_playerAim.ToggleAim == true)
                _playerCamera.UnAimScope();
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

        if (_playerAim.ToggleAim == true)
            _playerCamera.AimScope();
    }

    public void DoneReloadMag()
    {
        animator.SetBool("Reload_Mag", false);
        animator.SetBool("Reload_Single", true);
    }

    public void ABC()
    {
        Debug.Log("IDK");
    }
}
