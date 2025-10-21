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

public class PlayerEvents
{
    public event Action<bool> OnAimStateChanged;
    public event EventHandler<WeaponEventArgs> OnWeaponChanged;
    public class WeaponEventArgs : EventArgs
    {
        public GameObject CurrentWeapon;
        public GunType GunType;
    }

    /// <summary>
    /// true = đang ngắm, false = thôi ngắm
    /// </summary>
    /// <param name="isAiming"></param>
    public void InvokeAimStateChanged(bool isAiming)
    {
        OnAimStateChanged?.Invoke(isAiming);
    }

    public void InvokeWeaponChanged(GameObject currentWeapon, GunType gunType)
    {
        OnWeaponChanged?.Invoke(this, new WeaponEventArgs
        {
            CurrentWeapon = currentWeapon,
            GunType = gunType
        });
    }
}

public class PlayerRoot : NetworkBehaviour
{
    #region References
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
    #endregion

    public PlayerEvents Events { get; private set; }

    void Awake()
    {
        ReferenceAssignment();
        Events = new PlayerEvents();
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

    GameObject FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
                return child.gameObject;

            // Nếu không phải, tìm trong con của con
            var found = FindChildWithTag(child, tag);
            if (found != null)
                return found;
        }
        return null;
    }

    void ReferenceAssignment()
    {
        if (TryGetComponent<ClientNetworkTransform>(out var clientNetworkTransform)) ClientNetworkTransform = clientNetworkTransform;
        if (TryGetComponent<PlayerInput>(out var playerInput)) PlayerInput = playerInput;
        if (TryGetComponent<CharacterController>(out var characterController)) CharacterController = characterController;
        if (TryGetComponent<PlayerNetwork>(out var playerNetwork)) PlayerNetwork = playerNetwork;
        if (TryGetComponent<PlayerAssetsInputs>(out var playerAssetsInputs)) PlayerAssetsInputs = playerAssetsInputs;
        if (TryGetComponent<PlayerTakeDamage>(out var playerTakeDamage)) PlayerTakeDamage = playerTakeDamage;
        if (TryGetComponent<PlayerShoot>(out var playerShoot)) PlayerShoot = playerShoot;
        if (TryGetComponent<PlayerController>(out var playerController)) PlayerController = playerController;
        if (TryGetComponent<PlayerUI>(out var playerUI)) PlayerUI = playerUI;
        if (TryGetComponent<PlayerInteract>(out var playerInteract)) PlayerInteract = playerInteract;
        if (TryGetComponent<PlayerInventory>(out var playerInventory)) PlayerInventory = playerInventory;
        if (TryGetComponent<PlayerReload>(out var playerReload)) PlayerReload = playerReload;
        if (TryGetComponent<PlayerAim>(out var playerAim)) PlayerAim = playerAim;
        if (TryGetComponent<PlayerCamera>(out var playerCamera)) PlayerCamera = playerCamera;
        if (TryGetComponent<PlayerCollision>(out var playerCollision)) PlayerCollision = playerCollision;
        if (TryGetComponent<PlayerLook>(out var playerLook)) PlayerLook = playerLook;

        if (FindChildWithTag(transform, "WeaponHolder") != null)
        {
            if (FindChildWithTag(transform, "WeaponHolder").TryGetComponent<WeaponHolder>(out var weaponHolder)) WeaponHolder = weaponHolder;
        }
        else
        {
            Debug.Log("Không tìm được WeaponHolder Object");
        }

        if (FindChildWithTag(transform, "PlayerModel") != null)
        {
            if (FindChildWithTag(transform, "PlayerModel").TryGetComponent<PlayerModel>(out var playerModel)) PlayerModel = playerModel;
        }
        else
        {
            Debug.Log("Không tìm được PlayerModel Object");
        }
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