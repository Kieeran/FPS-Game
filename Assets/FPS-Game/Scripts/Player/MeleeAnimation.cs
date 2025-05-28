using Unity.Netcode;
using UnityEngine;

public class MeleeAnimation : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public Animator animator;

    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool queuedAttackB = false;
    [HideInInspector]
    public bool mouseButtonUp = false;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    void OnEnable()
    {
        animator.Rebind();
        animator.Update(0f);

        isAttacking = false;
        queuedAttackB = false;
        mouseButtonUp = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerTakeDamage.IsPlayerDead) return;

        if (!isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack_1");
                isAttacking = true;
                queuedAttackB = false;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                animator.SetTrigger("Attack_4");
                isAttacking = true;
            }
        }
        else
        {
            if (!mouseButtonUp)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    mouseButtonUp = true;
                }

                if (Input.GetMouseButton(0))
                {
                    queuedAttackB = true;
                }
            }

            else
            {
                queuedAttackB = false;
            }
        }
    }

    public void OnAttackAEnd()
    {
        if (queuedAttackB)
        {
            Debug.Log("Attack_2");
            animator.SetTrigger("Attack_2");
            queuedAttackB = false;
        }
        else
        {
            Debug.Log("Attack_2");
            EndAttack();
        }
    }

    public void OnAttackBOrCEnd()
    {
        EndAttack();
    }

    void EndAttack()
    {
        isAttacking = false;
        mouseButtonUp = false;
        animator.ResetTrigger("Attack_1");
        animator.ResetTrigger("Attack_2");
        animator.ResetTrigger("Attack_4");
    }
}