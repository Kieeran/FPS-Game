using UnityEngine;
using Unity.Netcode;

public class PlayerBehaviour : NetworkBehaviour, IInitAwake, IInitStart, IInitNetwork
{
    protected PlayerRoot PlayerRoot { get; private set; }

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
    public virtual void InitializeStart()
    {

    }
    public virtual int PriorityNetwork => 1000;
    public virtual void InitializeOnNetworkSpawn()
    {

    }
}