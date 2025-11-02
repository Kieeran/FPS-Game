using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : PlayerBehaviour
{
    public Animator Animator { get; private set; }
    public RigBuilder RigBuilder;

    public override void InitializeOnNetworkSpawn()
    {
        base.InitializeOnNetworkSpawn();

        if (!IsOwner) return;
        Animator = GetComponent<Animator>();
        if (PlayerRoot.IsCharacterBot()) return;
        PlayerRoot.Events.OnPlayerDead += () =>
        {
            UpdateRigBuilder_ServerRPC(false);
        };

        PlayerRoot.Events.OnPlayerRespawn += () =>
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
