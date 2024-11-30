using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Melee : Weapon
{
    [SerializeField] private Animator animator;
    private PlayerAssetsInputs playerAssetsInputs;

    private void Start()
    {
        playerAssetsInputs = PlayerInput.Instance.GetPlayerAssetsInputs();
    }

    public void SetBoolFalse()
    {
        animator.SetBool("IsAttacking", false);
        animator.Play("IsAttacking", 0, 0f);
    }

    private void Update()
    {
        if (playerAssetsInputs.shoot == true)
        {
            if (animator.GetBool("IsAttacking") == false)
            {
                animator.SetBool("IsAttacking", true);
            }

            playerAssetsInputs.shoot = false;
        }
    }
}