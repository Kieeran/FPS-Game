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

    // public Image GetEscapeUI() { return escapeUI; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        quitGameButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();

                GameSceneManager.Instance.LoadPreviousScene();
            }
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
    }
}