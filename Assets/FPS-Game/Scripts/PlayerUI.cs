using PlayerAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] Image _escapeUI;
    [SerializeField] Button _quitGameButton;
    [SerializeField] GameObject _scoreBoard;

    [SerializeField] Transform _container;

    [SerializeField] WeaponHud _weaponHud;

    public WeaponHud GetWeaponHud() { return _weaponHud; }

    // public Image GetEscapeUI() { return escapeUI; }

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