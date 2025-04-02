using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    [SerializeField] private Image escapeUI;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private GameObject scoreBoard;

    [SerializeField] private Transform container;

    // public Image GetEscapeUI() { return escapeUI; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!LobbyManager.Instance.IsLobbyHost()) quitGameButton.gameObject.SetActive(false);

        quitGameButton.onClick.AddListener(() =>
        {
            if (IsOwner == false) return;

            // Gửi sự kiện cho tất cả Client để xử lý thoát game
            NotifyClientsToQuit_ServerRpc();

            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.ExitGame();
            GameSceneManager.Instance.LoadPreviousScene();
        });
    }

    // RPC để thông báo Client thoát
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
        if (playerAssetsInputs.escapeUI == true)
        {
            escapeUI.gameObject.SetActive(!escapeUI.gameObject.activeSelf);

            Cursor.lockState = !escapeUI.gameObject.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;

            playerAssetsInputs.escapeUI = false;
        }

        if (playerAssetsInputs.openScoreboard == true)
        {
            scoreBoard.gameObject.SetActive(!scoreBoard.gameObject.activeSelf);

            playerAssetsInputs.openScoreboard = false;
        }
    }
}