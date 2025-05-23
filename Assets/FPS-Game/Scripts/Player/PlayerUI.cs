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
    public Image HealthUI { get; private set; }
    public Image HitEffect { get; private set; }
    public Image ScopeAim { get; private set; }

    [SerializeField] float _hitBodyAlpha;
    [SerializeField] float _hitHeadAlpha;

    [SerializeField] Canvas _playerUI;
    [SerializeField] Image _escapeUI;
    [SerializeField] Button _quitGameButton;
    [SerializeField] GameObject _scoreBoard;
    [SerializeField] GameObject _bulletHud;
    [SerializeField] WeaponHud _weaponHud;
    [SerializeField] Image _crossHair;

    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    TextMeshProUGUI _ammoInfo;
    ReloadEffect _reloadEffect;

    public Action OnOpenScoreBoard;

    public WeaponHud GetWeaponHud() { return _weaponHud; }

    public void StartReloadEffect(System.Action onDone)
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

    public void UpdatePlayerHealthBar(float amount)
    {
        HealthUI.fillAmount = amount;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        Instantiate(_playerUI, transform);

        if (!LobbyManager.Instance.IsLobbyHost()) _quitGameButton.gameObject.SetActive(false);

        _quitGameButton.onClick.AddListener(() =>
        {
            if (IsOwner == false) return;

            // Gửi sự kiện cho tất cả Client để xử lý thoát game
            NotifyClientsToQuit_ServerRpc();

            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.ExitGame();

            // GameSceneManager.Instance.LoadPreviousScene();
            GameSceneManager.Instance.LoadScene("Lobby Room");
        });

        PlayerRoot.PlayerAim.OnAim += () =>
        {
            _crossHair.gameObject.SetActive(false);
        };

        PlayerRoot.PlayerAim.OnUnAim += () =>
        {
            _crossHair.gameObject.SetActive(true);
        };
    }

    [ServerRpc]
    private void NotifyClientsToQuit_ServerRpc()
    {
        NotifyClientsToQuit_ClientRpc();
    }

    [ClientRpc]
    private void NotifyClientsToQuit_ClientRpc()
    {
        // Hành động cho từng Client khi host thoát
        if (!IsOwner)
        {
            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.ExitGame();

            // GameSceneManager.Instance.LoadPreviousScene();
            GameSceneManager.Instance.LoadScene("Lobby Room");
        }
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
            _escapeUI.gameObject.SetActive(!_escapeUI.gameObject.activeSelf);

            Cursor.lockState = !_escapeUI.gameObject.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;

            PlayerRoot.PlayerAssetsInputs.escapeUI = false;
        }

        if (PlayerRoot.PlayerAssetsInputs.openScoreboard == true)
        {
            _scoreBoard.SetActive(!_scoreBoard.activeSelf);

            if (_scoreBoard.activeSelf == true)
                OnOpenScoreBoard?.Invoke();
            else
                ClearScoreBoard();

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

    void ClearScoreBoard()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    public void UpdateUIClientRpc(float alpha, ClientRpcParams clientRpcParams)
    {
        StartCoroutine(FadeHitEffect(HitEffect, alpha));
    }

    private IEnumerator FadeHitEffect(Image hitEffect, float targetAlpha)
    {
        float currentAlpha = targetAlpha;

        while (currentAlpha > 0)
        {
            hitEffect.color = new Color(1, 0, 0, currentAlpha);
            currentAlpha -= Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateUI(float damage, ulong targetClientId)
    {
        if (damage == 0.05f)
        {
            UpdateUIServerRpc(_hitBodyAlpha / 255, targetClientId);
        }
        else if (damage == 0.1f)
        {
            UpdateUIServerRpc(_hitHeadAlpha / 255, targetClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateUIServerRpc(float alpha, ulong targetClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { targetClientId }
            }
        };

        PlayerRoot.PlayerUI.UpdateUIClientRpc(alpha, clientRpcParams);
    }
}