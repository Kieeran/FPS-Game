using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    public PlayerCanvas CurrentPlayerCanvas { get; private set; }

    [SerializeField] PlayerCanvas _playerCanvas;
    [SerializeField] GameObject _bulletHud;
    [SerializeField] WeaponHud _weaponHud;
    [SerializeField] Image _crossHair;

    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    TextMeshProUGUI _ammoInfo;
    ReloadEffect _reloadEffect;

    public Action OnOpenScoreBoard;

    public WeaponHud GetWeaponHud() { return _weaponHud; }

    public void StartReloadEffect(Action onDone)
    {
        _reloadEffect.StartReloadEffect(() =>
        {
            onDone?.Invoke();
        });
    }

    void Awake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();

        _reloadEffect = _bulletHud.transform.Find("Reload").GetComponent<ReloadEffect>();

        Transform ammoInfoTransform = _bulletHud.transform.Find("BulletAmount/AmmoInfo");
        if (ammoInfoTransform != null)
        {
            _ammoInfo = ammoInfoTransform.GetComponent<TextMeshProUGUI>();
            if (_ammoInfo == null)
            {
                Debug.LogError("Không tìm thấy TextMeshPro trong AmmoInfo!");
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy BulletAmount/AmmoInfo trong Bullet HUD!");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        CurrentPlayerCanvas = Instantiate(_playerCanvas, transform);

        PlayerRoot.PlayerAim.OnAim += () =>
        {
            _crossHair.gameObject.SetActive(false);
        };

        PlayerRoot.PlayerAim.OnUnAim += () =>
        {
            _crossHair.gameObject.SetActive(true);
        };
    }



    public void SetAmmoInfoUI(int currentMagazineAmmo, int totalAmmo)
    {
        _ammoInfo.text = currentMagazineAmmo.ToString() + "/" + totalAmmo.ToString();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.escapeUI == true)
        {
            CurrentPlayerCanvas.ToggleEscapeUI();
            PlayerRoot.PlayerAssetsInputs.escapeUI = false;
        }

        if (PlayerRoot.PlayerAssetsInputs.openScoreboard == true)
        {
            CurrentPlayerCanvas.ToggleScoreBoard();
            PlayerRoot.PlayerAssetsInputs.openScoreboard = false;
        }
    }

    public void AddInfoToScoreBoard(List<PlayerNetwork.PlayerInfo> playerInfos)
    {
        foreach (PlayerNetwork.PlayerInfo playerInfo in playerInfos)
        {
            GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
            if (itemGO.TryGetComponent<PlayerScoreboardItem>(out var item))
            {
                item.Setup(playerInfo.Name + "", playerInfo.KillCount, playerInfo.DeathCount);
            }
        }
    }
}