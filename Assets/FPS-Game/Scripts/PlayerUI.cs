using PlayerAssets;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] Image _escapeUI;
    [SerializeField] Button _quitGameButton;
    [SerializeField] GameObject _scoreBoard;
    [SerializeField] GameObject _bulletHud;
    [SerializeField] WeaponHud _weaponHud;

    TextMeshProUGUI _ammoInfo;
    ReloadEffect _reloadEffect;

    public WeaponHud GetWeaponHud() { return _weaponHud; }

    public void StartReloadEffect(System.Action onDone)
    {
        _reloadEffect.StartReloadEffect(() =>
        {
            onDone?.Invoke();
        });
    }

    // public Image GetEscapeUI() { return escapeUI; }

    void Awake()
    {
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

        if (!LobbyManager.Instance.IsLobbyHost()) _quitGameButton.gameObject.SetActive(false);

        _quitGameButton.onClick.AddListener(() =>
        {
            if (IsOwner == false) return;

            // Gửi sự kiện cho tất cả Client để xử lý thoát game
            NotifyClientsToQuit_ServerRpc();

            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.ExitGame();
            GameSceneManager.Instance.LoadPreviousScene();
        });
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
            GameSceneManager.Instance.LoadPreviousScene();
        }
    }

    public void SetAmmoInfoUI(int currentMagazineAmmo, int totalAmmo)
    {
        _ammoInfo.text = currentMagazineAmmo.ToString() + "/" + totalAmmo.ToString();
    }

    void Update()
    {
        if (_playerAssetsInputs.escapeUI == true)
        {
            _escapeUI.gameObject.SetActive(!_escapeUI.gameObject.activeSelf);

            Cursor.lockState = !_escapeUI.gameObject.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;

            _playerAssetsInputs.escapeUI = false;
        }

        if (_playerAssetsInputs.openScoreboard == true)
        {
            _scoreBoard.gameObject.SetActive(!_scoreBoard.gameObject.activeSelf);

            _playerAssetsInputs.openScoreboard = false;
        }
    }
}