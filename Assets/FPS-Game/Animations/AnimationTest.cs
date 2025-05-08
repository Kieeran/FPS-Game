using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public Animator animator;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && animator.GetBool("Shoot") == false &&
        animator.GetBool("Reload_Single") == false)
        {
            animator.SetBool("Shoot", true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetBool("Reload_Mag", true);
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
