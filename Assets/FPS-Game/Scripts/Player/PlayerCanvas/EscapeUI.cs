using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : NetworkBehaviour
{
    [SerializeField] Image _escapeUI;
    [SerializeField] Button _quitGameButton;

    void Awake()
    {
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
}
