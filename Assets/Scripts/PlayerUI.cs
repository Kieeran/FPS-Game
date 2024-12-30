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

    // public Image GetEscapeUI() { return escapeUI; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        quitGameButton.onClick.AddListener(() =>
        {
            if (IsOwner == false) return;

            NetworkManager.Singleton.Shutdown();
            GameSceneManager.Instance.LoadPreviousScene();
        });
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