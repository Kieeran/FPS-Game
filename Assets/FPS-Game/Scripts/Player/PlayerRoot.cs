using System;
using System.Collections.Generic;
using System.Linq;
using PlayerAssets;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Priority = 1000 => không ảnh hưởng bởi thứ tự ưu tiên
/// </summary>

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

public class PlayerRoot : NetworkBehaviour
{
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
    public PlayerCollision PlayerCollision { get; private set; }
    public PlayerLook PlayerLook { get; private set; }
    public PlayerModel PlayerModel { get; private set; }
    public WeaponHolder WeaponHolder { get; private set; }

    [SerializeField] WeaponHolder _weaponHolder;
    [SerializeField] PlayerModel _playerModel;

    void Awake()
    {
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
        PlayerCollision = GetComponent<PlayerCollision>();
        PlayerLook = GetComponent<PlayerLook>();

        WeaponHolder = _weaponHolder;
        PlayerModel = _playerModel;

        InitAwake(gameObject);
    }

    void Start()
    {
        InitStart(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        InitOnNetworkSpawn(gameObject);
    }

    void InitByPriorityInRootInterface<TInterface, TPriority>(
    GameObject root,
    Func<TInterface, TPriority> getPriority,
    Action<TInterface> initMethod) where TInterface : class
    {
        var allMonoBehaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
        var list = allMonoBehaviours
            .OfType<TInterface>()   // Lọc component có implement interface TInterface
            .ToList();

        list.Sort((a, b) => Comparer<TPriority>.Default.Compare(getPriority(a), getPriority(b)));

        foreach (var item in list)
            initMethod(item);
    }

    void InitAwake(GameObject root)
    {
        InitByPriorityInRootInterface<IInitAwake, int>(
            root,
            x => x.PriorityAwake,
            x => x.InitializeAwake()
        );
    }

    void InitStart(GameObject root)
    {
        InitByPriorityInRootInterface<IInitStart, int>(
            root,
            x => x.PriorityStart,
            x => x.InitializeStart()
        );
    }

    void InitOnNetworkSpawn(GameObject root)
    {
        InitByPriorityInRootInterface<IInitNetwork, int>(
            root,
            x => x.PriorityNetwork,
            x => x.InitializeOnNetworkSpawn()
        );
    }

    // // Awake
    // public int PriorityAwake => -1;
    // public void InitializeAwake()
    // {

    // }

    // // Start
    // public int PriorityStart => -1;
    // public void InitializeStart()
    // {

    // }

    // // OnNetworkSpawn
    // public int PriorityNetwork => -1;
    // public void InitializeOnNetworkSpawn()
    // {

    // }
}