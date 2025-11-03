using UnityEngine;
using Unity.Netcode;

public class PlayerBehaviour : NetworkBehaviour, IInitAwake, IInitStart, IInitNetwork, IWaitForInGameManager
{
    protected PlayerRoot PlayerRoot { get; private set; }
    public PlayerRoot GetPlayerRoot() { return PlayerRoot; }

    public virtual int PriorityAwake => 1000;
    public virtual void InitializeAwake()
    {
        if (transform.root.TryGetComponent<PlayerRoot>(out var playerRoot))
        {
            PlayerRoot = playerRoot;
        }
        else
        {
            PlayerRoot = null;
            Debug.Log("Không tìm thấy PlayerRoot");
        }
    }
    public virtual int PriorityStart => 1000;
    public virtual void InitializeStart() { }
    public virtual int PriorityNetwork => 1000;
    public virtual void InitializeOnNetworkSpawn()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(InGameManagerWaiter.WaitForInGameManager(this));
    }
    public virtual void OnInGameManagerReady(InGameManager manager) { }
}