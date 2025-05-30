using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    public PlayerAnimation PlayerAni { get; private set; }
    public List<Renderer> headParts;

    void Awake()
    {
        PlayerAni = GetComponent<PlayerAnimation>();
    }

    public void HideHead()
    {
        foreach (var part in headParts)
        {
            part.enabled = false;
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
        Debug.Log("Die animation");

        PlayerAni.Animator.Play("FallingForwardDeath", 0, 0f);
    }

    public void OnPlayerRespawn()
    {
        Debug.Log("Restart animation");

        PlayerAni.Animator.Play("Idle Walk Run Blend", 0, 0f);
        transform.localPosition = Vector3.zero;
    }
}
