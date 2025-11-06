using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CSharp;
using PlayerAssets;
using Unity.Collections;
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

public class PlayerEvents
{
    public class WeaponEventArgs : EventArgs
    {
        public GameObject CurrentWeapon;
        public GunType GunType;
    }

    #region Events
    public event Action<bool> OnAimStateChanged;
    public event EventHandler<WeaponEventArgs> OnWeaponChanged;
    public event Action OnPlayerRespawn;
    public event Action OnPlayerDead;
    public event Action ToggleEscapeUI;
    public event Action OnReload;
    public event Action OnCollectedHealthPickup;

    public event Action OnLeftSlash_1;
    public event Action OnLeftSlash_2;
    public event Action OnRightSlash;
    public event Action OnDoneSlash;
    public event Action OnCheckSlashHit;

    public event Action OnQuitGame;
    #endregion

    #region Invoke Events
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

    public void InvokeOnPlayerRespawn()
    {
        // Event được kích hoạt ở local
        OnPlayerRespawn?.Invoke();
    }

    public void InvokeOnPlayerDead()
    {
        // Event được kích hoạt ở local
        OnPlayerDead?.Invoke();
    }

    public void InvokeToggleEscapeUI()
    {
        ToggleEscapeUI?.Invoke();
    }

    public void InvokeOnReload()
    {
        OnReload?.Invoke();
    }

    public void InvokeOnCollectedHealthPickup()
    {
        OnCollectedHealthPickup?.Invoke();
    }

    public void InvokeOnLeftSlash_1()
    {
        OnLeftSlash_1?.Invoke();
    }

    public void InvokeOnLeftSlash_2()
    {
        OnLeftSlash_2?.Invoke();
    }

    public void InvokeOnRightSlash()
    {
        OnRightSlash?.Invoke();
    }

    public void InvokeOnDoneSlash()
    {
        OnDoneSlash?.Invoke();
    }

    public void InvokeOnCheckSlashHit()
    {
        OnCheckSlashHit?.Invoke();
    }

    public void InvokeOnQuitGame()
    {
        OnQuitGame?.Invoke();
    }
    #endregion
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
    public AIInputFeeder AIInputFeeder { get; private set; }
    #endregion

    public PlayerEvents Events { get; private set; }
    public NetworkVariable<bool> IsBot = new();
    public NetworkVariable<FixedString32Bytes> BotID = new();
    public void SetIsCharacterBot(bool b)
    {
        if (!IsServer)
        {
            Debug.Log("Không phải server/host, không thể chuyển trạng thái IsBot");
            return;
        }
        IsBot.Value = b;
    }
    public bool IsCharacterBot() { return IsBot.Value; }
    public string GetBotID() { return BotID.Value.ToString(); }
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
        if (TryGetComponent<AIInputFeeder>(out var aIInputFeeder)) AIInputFeeder = aIInputFeeder;

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


    /// <summary>
    /// Priority = 1000 => không ảnh hưởng bởi thứ tự ưu tiên
    /// </summary>

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