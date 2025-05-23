using PlayerAssets;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInitAwake
{
    int PriorityAwake { get; }
    void InitializeAwake();
}

public interface IInitStart
{
    int PriorityStart { get; }
    void InitializeStart();
}

public interface IInitNetwork
{
    int PriorityNetwork { get; }
    void InitializeOnNetworkSpawn();
}

public class PlayerRoot : MonoBehaviour
{
    public NetworkObject NetworkObject { get; private set; }
    public ClientNetworkTransform ClientNetworkTransform { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public PlayerNetwork PlayerNetwork { get; private set; }
    public PlayerAssetsInputs PlayerAssetsInputs { get; private set; }
    public PlayerTakeDamage PlayerTakeDamage { get; private set; }
    public PlayerShoot PlayerShoot { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public PlayerUI PlayerUI { get; private set; }
    public PlayerInteract PlayerInteract { get; private set; }
    public PlayerInventory PlayerInventory { get; private set; }
    public PlayerReload PlayerReload { get; private set; }
    public PlayerAim PlayerAim { get; private set; }
    public PlayerCamera PlayerCamera { get; private set; }

    void Awake()
    {
        NetworkObject = GetComponent<NetworkObject>();
        ClientNetworkTransform = GetComponent<ClientNetworkTransform>();
        PlayerInput = GetComponent<PlayerInput>();
        CharacterController = GetComponent<CharacterController>();
        PlayerNetwork = GetComponent<PlayerNetwork>();
        PlayerAssetsInputs = GetComponent<PlayerAssetsInputs>();
        PlayerTakeDamage = GetComponent<PlayerTakeDamage>();
        PlayerShoot = GetComponent<PlayerShoot>();
        PlayerController = GetComponent<PlayerController>();
        PlayerUI = GetComponent<PlayerUI>();
        PlayerInteract = GetComponent<PlayerInteract>();
        PlayerInventory = GetComponent<PlayerInventory>();
        PlayerReload = GetComponent<PlayerReload>();
        PlayerAim = GetComponent<PlayerAim>();
        PlayerCamera = GetComponent<PlayerCamera>();
    }
}