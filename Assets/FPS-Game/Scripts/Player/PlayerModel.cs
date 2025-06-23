using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    public PlayerAnimation PlayerAni { get; private set; }
    public List<Renderer> modelParts;

    void Awake()
    {
        PlayerAni = GetComponent<PlayerAnimation>();
    }

    public void DisableModel()
    {
        foreach (var part in modelParts)
        {
            part.enabled = false;
        }
    }

    public void EnableModel()
    {
        foreach (var part in modelParts)
        {
            part.enabled = true;
        }
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

        EnableModel();
    }

    public void OnPlayerRespawn()
    {
        if (!IsOwner) return;
        Debug.Log("Restart animation");

        PlayerAni.Animator.Play("Idle and Run", 0, 0f);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        PlayerAni.Animator.applyRootMotion = false;

        DisableModel();
    }
}
