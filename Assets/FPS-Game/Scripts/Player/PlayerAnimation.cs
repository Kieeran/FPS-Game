using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : NetworkBehaviour
{
    public PlayerRoot PlayerRoot;
    public Animator Animator { get; private set; }
    public RigBuilder RigBuilder;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;
        Animator = GetComponent<Animator>();

        PlayerRoot.PlayerTakeDamage.OnPlayerDead += () =>
        {
            UpdateRigBuilder_ServerRPC(false);
        };

        PlayerRoot.PlayerNetwork.OnPlayerRespawn += () =>
        {
            UpdateRigBuilder_ServerRPC(true);
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateRigBuilder_ServerRPC(bool b)
    {
        UpdateRigBuilder_ClientRPC(b);
    }

    [ClientRpc]
    public void UpdateRigBuilder_ClientRPC(bool b)
    {
        RigBuilder.enabled = b;
    }

    public void OnFootstep(AnimationEvent animationEvent)
    {
        PlayerRoot.PlayerController.OnFootstep(animationEvent);
    }

    public void OnLand(AnimationEvent animationEvent)
    {
        PlayerRoot.PlayerController.OnLand(animationEvent);
    }
}
