using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
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
