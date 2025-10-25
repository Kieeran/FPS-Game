using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerModel : PlayerBehaviour
{
    public PlayerAnimation PlayerAni { get; private set; }
    public List<Renderer> modelParts;
    public RigBuilder RigBuilder;
    public BoneRenderer BoneRenderer;

    public override void InitializeAwake()
    {
        base.InitializeAwake();
        PlayerAni = GetComponent<PlayerAnimation>();
    }

    public void ChangeModelVisibility(bool b)
    {
        foreach (var part in modelParts)
        {
            part.enabled = b;
        }
    }

    public void ChangeRigBuilderState(bool b)
    {
        RigBuilder.enabled = b;
        BoneRenderer.enabled = b;
    }

    void Update()
    {
        // if (!IsOwner) return;

        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     Debug.Log("Die animation");
        //     PlayerAni.Animator.Play("FallingForwardDeath", 0, 0f);
        // }

        // if (Input.GetKeyDown(KeyCode.N))
        // {
        //     Debug.Log("Restart animation");
        //     PlayerAni.Animator.Play("Idle Walk Run Blend", 0, 0f);

        //     transform.localPosition = Vector3.zero;
        // }
    }

    public void OnPlayerDie()
    {
        if (!IsOwner) return;
        Debug.Log("Die animation");
        PlayerAni.Animator.applyRootMotion = true;
        PlayerAni.Animator.Play("FallingForwardDeath", 0, 0f);

        ChangeModelVisibility(true);
    }

    public void OnPlayerRespawn()
    {
        if (!IsOwner) return;
        Debug.Log("Restart animation");

        PlayerAni.Animator.Play("Idle and Run", 0, 0f);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        PlayerAni.Animator.applyRootMotion = false;

        ChangeModelVisibility(false);
    }
}
