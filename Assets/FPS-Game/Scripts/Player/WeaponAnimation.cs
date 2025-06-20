using Unity.Netcode;
using UnityEngine;
public class WeaponAnimation : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public Animator animator;
    public bool Automatic;

    [HideInInspector]
    public bool IsShooting = false;
    [HideInInspector]
    public bool IsReloading = false;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    void OnEnable()
    {
        animator.Play("Reload", 0, 0f);
        animator.Play("Shoot", 0, 0f);

        animator.Rebind();
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);

        IsShooting = false;
        IsReloading = false;
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

    bool _isPressed = false;

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
            if (PlayerRoot.PlayerAssetsInputs.shoot == true && !IsShooting && !IsReloading && _isPressed == false)
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
        IsReloading = false;
    }

    public void ABC()
    {
        Debug.Log("IDK");
    }
}
